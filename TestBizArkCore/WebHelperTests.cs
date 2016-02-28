using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using BizArk.Core.Util;
using BizArk.Core.Web;
using NUnit.Framework;

namespace BizArk.Core.Tests
{


    /// <summary>
    ///This is a test class for NoContentTypeTest and is intended
    ///to contain all NoContentTypeTest Unit Tests
    ///</summary>
	[TestFixture]
    public class WebHelperTests
    {

        //[TestMethod]
        //public void WebParameterTest()
        //{
        //    dynamic parameters = new WebParameters();
        //    parameters.Test = 5;
        //    Assert.AreEqual("5", parameters.Test);
        //    AssertEx.Throws(typeof(RuntimeBinderException), () => { var x = parameters.Test2; });

        //    var txtParam = parameters.Find("Test") as WebTextParameter;
        //    Assert.IsNotNull(txtParam);
        //    Assert.AreEqual(txtParam.Name, "Test");
        //    Assert.AreEqual(txtParam.Value, "5");

        //    parameters.MyFile = new FileInfo(@"C:\MyFile.txt");
        //    var fileParam = parameters.Find("MyFile") as WebFileParameter;
        //    Assert.IsNotNull(fileParam);
        //    Assert.AreEqual(@"C:\MyFile.txt", fileParam.File.FilePath);

        //    parameters.MyData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        //    var binParam = parameters.Find("MyData") as WebBinaryParameter;
        //    Assert.IsNotNull(binParam);
        //    Assert.AreEqual(10, binParam.Data.Length);
        //    Assert.AreEqual(0, binParam.Data[0]);
        //    Assert.AreEqual(9, binParam.Data[9]);
        //}

        //[Test]
        //public void NoContentTypeUrlTest()
        //{
        //    var contentType = new NoContentType(new WebParameters());
        //    var helper = new WebHelper("http://redwerb.com");
        //    var url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com", url);

        //    dynamic parameters = new WebParameters();
        //    parameters.Test = "hello";
        //    contentType = ContentType.CreateContentType(HttpMethod.Get, parameters) as NoContentType;
        //    Assert.IsNotNull(contentType);
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com?Test=hello", url);

        //    helper = new WebHelper("http://redwerb.com?hello=world");
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com?hello=world&Test=hello", url);
        //}

        //[Test]
        //public void ContentTypeUrlTest()
        //{
        //    var helper = new WebHelper("http://redwerb.com");
        //    var contentType = new ApplicationUrlEncodedContentType(helper.Parameters);

        //    var url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com", url);

        //    helper.Parameters.Test = "hello";
        //    contentType = ContentType.CreateContentType(HttpMethod.Post, helper.Parameters) as ApplicationUrlEncodedContentType;
        //    Assert.IsNotNull(contentType);
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com", url);

        //    helper = new WebHelper("http://redwerb.com?hello=world");
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com?hello=world", url);
        //}

        //[TestMethod]
        //public void CompressedResponseTest()
        //{
        //    // should test deflate compression.
        //    var helper = new WebHelper("http://redwerb.com");
        //    var response = helper.MakeRequest();
        //    var result = response.ResultToString();
        //    Assert.IsTrue(result.Contains("<h2>Tools, tips, and techniques for developers</h2>"));

        //    // should test gzip compression.
        //    helper = new WebHelper("http://bing.com");
        //    helper.Parameters.q = "redwerb";
        //    response = helper.MakeRequest();
        //    result = response.ResultToString();
        //    Assert.IsTrue(result.Contains("<title>redwerb - Bing</title>"));

        //    // should test no compression.
        //    helper = new WebHelper("http://redwerb.com");
        //    helper.UseCompression = false;
        //    response = helper.MakeRequest();
        //    result = response.ResultToString();
        //    Assert.IsTrue(result.Contains("<h2>Tools, tips, and techniques for developers</h2>"));
        //}

        //[TestMethod]
        //public void UploadFileTest()
        //{
        //    // This test only works when the test web project is running (not included in the BizArk test suite).
        //    if (false)
        //    {
        //        var helper = new WebHelper("http://localhost:49745/API/File/Upload");
        //        helper.Parameters.Name = "TestFile";
        //        helper.Parameters.Directory = "TestDir";
        //        helper.Parameters.File = new UploadFile("text/plain", @"D:\Test.txt");
        //        var response = helper.MakeRequest();
        //        var content = response.ResultToString();
        //        Assert.AreEqual("hello world!", content);
        //    }
        //}

[Test]
        public void MakeRequestStringTest()
        {
            var response = WebHelper.MakeRequest("http://www.google.com");
            var str = response.ResultToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(str));

            // Try with a different charset.
            response = WebHelper.MakeRequest("http://www.google.co.jp/");
            str = response.ResultToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(str));
			Assert.IsTrue(str.IndexOf(@"広告掲載") > 0); // I don't read japanese, but I saw these characters on the web page
        }

[Test]
        public void MakeRequestXmlTest()
        {
            var response = WebHelper.MakeRequest("http://www.w3schools.com/xml/note.xml");
            var doc = response.ResultToXml();
            Assert.IsNotNull(doc);
        }

[Test]
        public void DownloadFileTest()
        {
            var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
            try
            {
                var url = new UrlBuilder();
                url.Protocol = "https";
                url.Host = "chart.googleapis.com";
                url.Path.Add("chart");
                url.Parameters.AddRange(new
                {
                    chs = "150x150",
                    cht = "qr",
                    chl = "http://bizark.codeplex.com",
                    choe = "UTF-8"
                });
                WebHelper.DownloadFile(url.ToUri(), tmp);

                var fi = new FileInfo(tmp);
                Assert.IsTrue(fi.Exists);
                Assert.IsTrue(fi.Length > 0);
                using (var img = Image.FromFile(tmp))
                {
                    Assert.AreEqual(150, img.Width);
                    Assert.AreEqual(150, img.Height);
                }
            }
            finally
            {
                if (File.Exists(tmp))
                {
                    Thread.Sleep(500); // make sure the file is released before we try to delete it.
                    File.Delete(tmp);
                }
            }
        }

[Test]
        public void MakeRequestDataTest()
        {
            var url = new UrlBuilder();
            url.Protocol = "https";
            url.Host = "chart.googleapis.com";
            url.Path.Add("chart");
            url.Parameters.Add("chs", "150x150");
            url.Parameters.Add("cht", "qr");
            url.Parameters.Add("chl", "http://bizark.codeplex.com");
            url.Parameters.Add("choe", "UTF-8");
            var response = WebHelper.MakeRequest(url.ToUri());
            var data = (byte[])response.Result;
            Assert.IsTrue(data.Length > 0);
            var ms = new MemoryStream(data);
            ms.Position = 0;
            using (var img = Image.FromStream(ms))
            {
                Assert.AreEqual(150, img.Width);
                Assert.AreEqual(150, img.Height);
            }
        }

		[Test]
		[Ignore]
        public void MakeRequestUploadValuesTest()
        {
            var result = WebHelper.MakeRequest("http://localhost:89/Test/UploadValues", new { intVal = 123, strVal = "ABC" });
            var str = result.ResultToString();
            Assert.AreEqual("123 ABC", str);
        }

		[Test]
		[Ignore]
        public void MakeRequestUploadFileTest()
        {
            using (var tmp = new TempFile("txt"))
            {
                tmp.Write("TEST");
                var result = WebHelper.MakeRequest("http://localhost:89/Test/UploadValues", new { intVal = 123, strVal = "ABC", file = new FileInfo(tmp.TempPath) });
                var str = result.ResultToString();
                Assert.AreEqual("123 ABC [TEST]", str);
            }
        }

[Test]
        public void MakeRequestAsyncTest()
        {
            var handled = false;
            var done = WebHelper.MakeRequestAsync("http://www.google.com", new WebHelperOptions()
                {
                    RequestComplete = (web, response, ex, cancelled) =>
                    {
                        if (ex != null)
                        {
                            // handle error
                        }
                        if (cancelled) return;

                        handled = true;
                        var str = response.ResultToString();
                        Assert.IsFalse(string.IsNullOrWhiteSpace(str));
                    }
                });
            done.WaitOne();

            Assert.IsTrue(handled);
        }

        private void Garb()
        {
            var web = new WebClient();

            var options = new WebHelperOptions();
            //options.
        }

    }

}
