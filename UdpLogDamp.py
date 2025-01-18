import socket
import select

# UDPサーバーの設定
UDP_IP = "0.0.0.0"  # すべてのインターフェースで受信
UDP_PORT = 5005     # 使用するポート番号（C#アプリと一致させる必要があります）

# ソケットの作成
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind((UDP_IP, UDP_PORT))
sock.setblocking(False)  # ノンブロッキングモードに設定

print(f"Listening on {UDP_IP}:{UDP_PORT}")

try:
    while True:
        ready = select.select([sock], [], [], 1.0)  # 1秒タイムアウト
        if ready[0]:
            data, addr = sock.recvfrom(1024)  # バッファサイズは1024バイト
            message = data.decode('utf-8')
            print(f"{message} from {addr}")
except KeyboardInterrupt:
    print("\nServer stopped by user")
finally:
    sock.close()