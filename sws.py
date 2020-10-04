from socket import *


class zsws:
    def __init__(self, _routing_table, _ip_address, _port, _buf_size=1024, _tcp_term_char='~'):
        self.routing_table = _routing_table
        self.ip_address = _ip_address
        self.port = _port
        self.buf_size = _buf_size
        self.tcp_term_char = _tcp_term_char
        self.__init_server()
    

    def __init_server(self):
        self.server_socket = socket(AF_INET, SOCK_STREAM)
        try:
            self.server_socket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
            self.server_socket.bind((self.ip_address, self.port))
            print(f'[INFO]: server started at address {self.ip_address}:{self.port}')
            while (True):
                soc, addr = self.__accept_conn()
                self.__handle_conn(soc, addr)
        except KeyboardInterrupt:
            print(f'[INFO]: server shutdown')
        except Exception as exc:
            print(f'[ERROR]: {exc}')
        self.server_socket.close()


    def __accept_conn(self):
        self.server_socket.listen(1)
        (client_socket, client_address) = self.server_socket.accept()
        print(f'[INFO]: client {client_address} connected')
        return client_socket, client_address
    

    def __handle_conn(self, client_socket, client_address):
        http_request = ''
        while True:
            chunck = client_socket.recv(self.buf_size)
            if not chunck:
                break
            else:
                chunck_str = chunck.decode()
                if chunck_str[-1] == self.tcp_term_char:
                    http_request += chunck_str[:-1]
                    break
                else:
                    http_request += chunck_str
        http_parts = http_request.split('\r\n')
        if len(http_parts) > 0:
            main_http_header = http_parts[0]
            main_http_header_parts = main_http_header.split(' ')
            # main_http_header[1]: http route
            # main_http_header[0]: http method
            http_response = self.routing_table[main_http_header_parts[1]][main_http_header_parts[0]](http_request)
            client_socket.sendall(http_response.encode())
            client_socket.shutdown(SHUT_WR)
            print(f'[INFO]: sent to client {client_address}')
            print('=' * 100)
            print(f'{http_response}')
            print('=' * 100)
        else:
            print(f'[ERROR]: bad request')