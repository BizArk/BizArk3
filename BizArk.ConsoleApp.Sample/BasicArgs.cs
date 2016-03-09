using BizArk.Core.Extensions.FormatExt;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BizArk.ConsoleApp.Sample
{
	public class BasicArgs
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Occupation { get; set; }
		public bool HasHair { get; set; }
		public string[] Children { get; set; }
	}
}
