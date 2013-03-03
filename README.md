BluePlumGit/GitlabTool
===========

本アプリケーションは、GitlabをWindows環境で使用するためのヘルパーアプリケーションです。
Gitの制御には、NGitライブラリを使用しC#コードのみで構成されておりGit操作を行うためmsysgitなどのシェルソフトは
別途インストールする必要はありません。
アプリケーションのGUI層は、WPFを使用しています。

Gitクライアントツールは、GitHub for Windowsの使用を想定しています。
これは、デザイナーなど非プログラマの人達が、TortoiseGit+MsysGitをインストールして使いこなすのはハードルが高い
と考えるからです。
GitHub for Windowsは、当然ながらGitHub以外のサーバーでの使用は考慮されていませんが、Gitのクライアントソフトと
しては、とても良く出来ています。
GitlabToolは、GitHub for WindowsのGitHub以外からは、クローンが行えないという制限を補完します。
GitlabToolで、Gitlabからリポジトリのクローンを行いGitHub for Windowsへ登録を行えば、GitHub for Windowsの
フル機能を利用することができます。
Gitの接続プロトコルは、httpを使用します。

Gitlabをオンプレミスで運用していて、非プログラマのメンバーのGit使用に悩んでいる方の解決を目指しています。
現在は、リポジトリのクローンだけを実装していますが、GitHub for Windowsに無く、あると便利な機能を
追加していく予定です。

(1).gitkeepファイルによる空フォルダの保持

(2)言語別の代表的な.gitignoreファイルの作成
