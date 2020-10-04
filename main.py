import sws
import ema_core_logic
import json


def main_post(http_req):
    # [DEBUG]: print full http request
    print('=' * 100)
    print(http_req)
    print('=' * 100)
    # prepare http header
    http_res = 'HTTP/1.1 200 OK\r\n'
    http_res += 'Content-Type: application/json\r\n'
    http_res += '\r\n'
    # http payload analysis
    json_payload = http_req.split('\r\n')[-1]
    json_obj = json.loads(json_payload)
    # [DEBUG]: print full json payload
    if (json_obj['api_func'] == 'model_gen') | (json_obj['api_func'] == 'dyn_filter'):
        _alg_settings = {}
        _alg_settings['api_func'] = json_obj['api_func']
        _alg_settings['hffr'] = json_obj['args']['settings']['filter_ratio']
        _alg_settings['sig_per'] = json_obj['args']['data']['x_vals'][-1]
        json_out_obj = {}
        if json_obj['api_func'] == 'model_gen':
            stars_arr, model_wave = ema_core_logic.ema_core(json_obj['args']['data']['y_vals'], _alg_settings)
            json_out_obj['star_arr'] = stars_arr
            json_out_obj['model_td_x'] = model_wave[0]
            json_out_obj['model_td_y'] = model_wave[1]
        elif json_obj['api_func'] == 'dyn_filter':
            model_wave = ema_core_logic.ema_core(json_obj['args']['data']['y_vals'], _alg_settings)
            json_out_obj['model_td_x'] = model_wave[0]
            json_out_obj['model_td_y'] = model_wave[1]
        http_res += json.dumps(json_out_obj)
    else:
        http_res += '{"RES": "INVALID PARAM (api_func)"}'
    return http_res


routing_table = {
    '/': {
        'POST': main_post
    }
}


# main code
z_server = sws.zsws(routing_table, '0.0.0.0', 9999, _tcp_term_char=';')