# SugorokuServer

すごろくのやつのサーバーサイドアプリです

## Tickets

Issue: #6

Pull request: #3, #5, #9, #10

## 実装詳細

### Program.cs

プログラムの最初に実行される関数 (Main) 及び、クライアントと接続したあとのサーバーの処理を非同期で行うTask (便利なThread的な 詳細は [この記事](https://qiita.com/Temarin/items/ff74d39ae1cfed89d1c5) とかがわかりやすいかな?) の生成を担当しています

### CreateTcpServerSocket.cs

使用するポート番号を投げるとサーバーがクライアントの接続を待機するソケットを、bind, listenを済ませた上で返す `CreateServerSocket(int port)` 関数があります

### AcceptTcpConnection.cs

サーバーがクライアントの接続を待つソケットを投げると、accept での待機状態となり、受け取ったクライアントソケットのインスタンスを返す `CreateClientSocket(Socket serverSocket)` 関数があります

### HandleClient.cs

クライアントからのメッセージを受け取ったら、このファイルの中の「クライアントから送信された処理実行要求メッセージを解析し、返信するメッセージを作り出す」関数 `MakeSendMessage(string receivedMessage)` にそのメッセージを投げます。

この関数で受信したメッセージが「プレイヤーの作成」を要求しているのか、はたまた「すごろくを振る」要求をしているのかを判定した上で、要求に合う関数を呼び出す分岐点の役割も果たしています。

その他の関数は上の分岐点から呼び出される、「すごろくを振る」などの関数です。必要があれば読んでみてください。

### SugorokuLibrary の呼び出し

文字の送受信の方法など、クライアントと共通な送受信文字列等の取り扱いなどについては `SugorokuLibrary/Protocol` にあります
