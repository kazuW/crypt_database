Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Cryptography
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Collections.Generic

Module Module1

    Private stockPriceList As ArrayList = New ArrayList()

    Sub Main()

        Dim resHtml As String = Nothing
        Dim getUrl As String = Nothing
        Dim tempData As String
        Dim strData() As String

        Dim tempDatetime As String
        Dim tempDate As DateTime
        Dim chk As Int16

        Dim args() As String
        Dim num As Integer

        args = Split(Command(), " ")
        '引数１：通貨ペア
        '引数２：取引所
        '引数３：年

        If args(0) = "help" Or args(0) = "HELP" Then
            Console.WriteLine("crypt_database.exe exchange pair year")
        End If

        ' 引数チェック
        num = args.Count

        If num < 3 Then
            Console.WriteLine("Error :less arguments")
            Exit Sub
        End If

        Dim tz As System.TimeZone = System.TimeZone.CurrentTimeZone
        Dim utcOffset As TimeSpan = tz.GetUtcOffset(DateTime.Now)

        tempDatetime = args(2) + "/01/01 00:00:00"

        chk = DateTime.TryParse(tempDatetime, tempDate)

        If chk = False Then
            Console.WriteLine("Error :can't recogniza 'year'")
            Exit Sub
        End If

        tempDate -= utcOffset

        Dim TimeNow As Long = (tempDate.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000000
        Dim timestamp As String = TimeNow.ToString

        getUrl = "https://api.cryptowat.ch/markets/" + args(0) + "/" + args(1) + "/ohlc?periods=86400&after=" + timestamp

        Console.WriteLine("download start!!")
        Console.WriteLine("URL = " + getUrl)

        Try
            Using wc As WebClient = New WebClient()
                wc.Encoding = System.Text.Encoding.GetEncoding("utf-8")
                resHtml = wc.DownloadString(getUrl)
            End Using
        Catch ex As Exception
            Console.WriteLine("Error :Check arguments!!")
            Exit Sub
        End Try

        Try
            Dim JsonObject As Object = JsonConvert.DeserializeObject(resHtml)
            Dim JsonObject1 As Object = JsonObject("result")
            Dim JsonObject2 As Object = JsonObject1("86400")
            Dim tempJsonObject As Object

            Dim jTokens As JEnumerable(Of JToken) = JsonObject2.Children

            For i = 0 To jTokens.Count - 1
                Dim price As StockPrice = New StockPrice()
                tempJsonObject = JsonObject2(i)
                tempData = JsonConvert.SerializeObject(tempJsonObject).Replace(Chr(&H22), "").Replace("[", "").Replace("]", "")
                strData = tempData.Split(",")

                Dim unixTime As Integer = CType(strData(0), Integer)
                Dim jst = New DateTime(1970, 1, 1).AddSeconds(unixTime).ToLocalTime()

                price.PriceDate = jst

                price.StartPrice = CType(strData(1), Single)
                price.HighPrice = CType(strData(2), Single)
                price.LowPrice = CType(strData(3), Single)
                price.EndPrice = CType(strData(4), Single)
                price.EndVolume = CType(strData(5), Single)

                stockPriceList.Add(price)

            Next

        Catch ex As Exception
            Console.WriteLine("Error :Check arguments!!")
            Exit Sub
            GoTo cont1
        End Try

cont1:


        stockPriceList.Sort()

        Dim tmpData As String
        Dim FileName As String = ".\" + args(1).ToUpper + "_" + args(0) + ".csv"


        Dim Fw As IO.StreamWriter

        Fw = New IO.StreamWriter(FileName, False, System.Text.Encoding.Default)

        For Each price As StockPrice In stockPriceList

            tmpData = ConstructLine(price.PriceDate.ToString("yyyy/MM/dd"), "," + price.StartPrice.ToString, "," + price.HighPrice.ToString,
                                             "," + price.LowPrice.ToString, "," + price.EndPrice.ToString, "," + price.EndVolume.ToString)
            Fw.WriteLine(tmpData)
        Next


        Fw.Close()

        Console.WriteLine("download end!!")


    End Sub

    Private Function ConstructLine(ByVal str1 As String, ByVal str2 As String, ByVal str3 As String, ByVal str4 As String,
                           ByVal str5 As String, ByVal str6 As String) As String
        Dim strData As String

        strData = str1

        strData = strData + str2
        strData = strData + str3
        strData = strData + str4
        strData = strData + str5
        strData = strData + str6

        Return strData

    End Function

End Module



Public Class StockPrice

    Implements System.IComparable

    Private _priceDate As DateTime
    Private _startPrice As String
    Private _highPrice As String
    Private _lowPrice As String
    Private _endPrice As String
    Private _endVolume As String
    Private _adjPrice As String

    '日付
    Public Property PriceDate() As DateTime
        Get
            Return Me._priceDate
        End Get
        Set(ByVal value As DateTime)
            Me._priceDate = value
        End Set
    End Property

    '始値
    Public Property StartPrice() As Single
        Get
            Return Me._startPrice
        End Get
        Set(ByVal value As Single)
            Me._startPrice = value
        End Set
    End Property

    '高値
    Public Property HighPrice() As Single
        Get
            Return Me._highPrice
        End Get
        Set(ByVal value As Single)
            Me._highPrice = value
        End Set
    End Property

    '安値
    Public Property LowPrice() As Single
        Get
            Return Me._lowPrice
        End Get
        Set(ByVal value As Single)
            Me._lowPrice = value
        End Set
    End Property

    '終値
    Public Property EndPrice() As Single
        Get
            Return Me._endPrice
        End Get
        Set(ByVal value As Single)
            Me._endPrice = value
        End Set
    End Property

    '出来高
    Public Property EndVolume() As Single
        Get
            Return Me._endVolume
        End Get
        Set(ByVal value As Single)
            Me._endVolume = value
        End Set
    End Property

    '調整値
    Public Property AdjPrice() As Single
        Get
            Return Me._adjPrice
        End Get
        Set(ByVal value As Single)
            Me._adjPrice = value
        End Set
    End Property


    '日付チェック
    Public Function CompareTo(ByVal other As Object) As Integer Implements System.IComparable.CompareTo

        If Not (Me.GetType() = other.GetType()) Then
            Throw New ArgumentException()
        End If

        Dim obj As StockPrice = CType(other, StockPrice)
        Return Me._priceDate.CompareTo(obj._priceDate)

    End Function



End Class