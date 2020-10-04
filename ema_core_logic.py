import numpy as np
from scipy.fft import fft, ifft
import math


def ema_core(input_signal, alg_settings):
    input_signal = np.array(input_signal)
    fft_params = fft(input_signal)
    N = len(fft_params)
    # take only postive fft terms
    if N % 2 == 0:
        fft_params = fft_params[1:int(N / 2)]
    else:
        fft_params = fft_params[1:int((N - 1) / 2 + 1)]
    fft_params = np.nan_to_num(fft_params)
    fft_params = fft_params_filter(fft_params, alg_settings['hffr'])
    if alg_settings['api_func'] == 'dyn_filter':
        return time_domain_ifft([(i + 1, x) for i, x in zip(range(len(fft_params)), fft_params)], alg_settings['sig_per'], N)
    elif alg_settings['api_func'] == 'model_gen':
        fft_params_threshold = np.max(np.abs(fft_params)) / 2.0
        stars_arr = [{
            'p_mag': 0.0,
            'p_angle': 0.0,
            'w': 0.0
        }]
        cur_complex_pos = 0.0 + 0.0j
        model_fft_params = []
        for i in range(len(fft_params)):
            if np.abs(fft_params[i]) >= fft_params_threshold:
                model_fft_params.append((i + 1, fft_params[i]))
                mag = (2 / N) * np.abs(fft_params[i])
                phase = np.angle([fft_params[i]])[0]
                cur_complex_pos += (mag * np.exp(1j * phase))
                stars_arr.append({
                    'p_mag': np.abs(cur_complex_pos),
                    'p_angel': np.angle(cur_complex_pos),
                    'w': (2 * np.pi / alg_settings['sig_per']) * (i + 1)
                })
        print(f'model fft_params: {model_fft_params}')
        return stars_arr, time_domain_ifft(model_fft_params, alg_settings['sig_per'], N)


def fft_params_filter(fft_params, filter_ratio):
    spec_size = len(fft_params)
    end_ponit = int(spec_size * filter_ratio)
    output = fft_params[0:end_ponit]
    return output


def time_domain_ifft(fft_params, sig_per, n_samples):
    td_x_ifft = np.linspace(0.0, sig_per, n_samples)
    td_y_ifft = list(np.zeros(len(td_x_ifft)))
    for i in range(len(fft_params)):
        dom_mag = (2 / n_samples) * np.abs(fft_params[i][1])
        phase_shift = np.angle([fft_params[i][1]])
        td_yi_ifft = [dom_mag * math.cos(2 * math.pi * fft_params[i][0] / sig_per * t + phase_shift) for t in td_x_ifft]
        td_y_ifft = [sum(x) for x in zip(td_y_ifft, td_yi_ifft)]
    return list(td_x_ifft), td_y_ifft