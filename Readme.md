# Webリモコン
## 概要
Webブラウザからキーボード入力のエミュレーションができます。

## インストール
このアプリケーションはWebサーバを起動します。使う前に管理者権限のPowerShellを起動して

```shell
netsh http add urlacl url=http://+:12255/ user=Everyone
```
というコマンドを実行してください。

他にもファイアーウォールが設定されてないとスマートフォンなどから入力ができないのでTCP 12255のポートを許可してください。

以下工事中

# 使い方
起動するとhttp://localhost:12255/page/default.html にアクセスするとボタンが表示されるのでそれを押すとキーボードエミュレーションできます。

default.htmlを編集することで他のキーボードエミュレーションができます。

WebSocketを使って通信しています。localhost:12255/pageにWebSocketで接続して任意の文字列を送信するとそれをキーボードエミュレーションします。
送る文字列はhttps://docs.microsoft.com/ja-jp/dotnet/api/system.windows.forms.sendkeys.send?view=netcore-3.1 を参考にしてください。

pageフォルダ以下にhtmlファイルを配置して、ウィンドウ名と配置したhtmlを指定すると自動的にwebブラウザのページが切り替わるようにすることもできます。(javascriptで処理を書かないとだめですが・・・

# ダウンロード
[こちら](https://github.com/kousokujin/WebRemokon/releases/tag/beta)から
# Lisense
Copyright (c) 2020 kousokujin.

Released under the MIT license.
