
crypt_database  

##requirement  

Windows(.Net Framework4.5)  

##usages  

Cryptowatchから日次データ(OHLC)をダウンロードします。
exeとDLLファイルを同一ディレクトリーにコピーして使用してください。

##commandline

crypt_database.exe exchange currencypair year
ex)
crypt_database.exe bitflyer btcjpy 2017  : 2017/1/1からBitflyerのBTC-JPY日次データをダウンロード
パラメータはcryptowatchのドキュメントを参照してください。
cryptowatchのドキュメント-->https://cryptowatch.jp/docs

##file lists

crypt_database.exe : binary file
Newtonsoft.Json.dll : DLL file for Json
Module1.vb : Visual basic source file
