using System;
using System.ComponentModel.DataAnnotations;

namespace TableDataBaseMVC.Models
{
	public class ConnectionModel
	{
		[Display(Name = "Connection string")]
		public string ConnectionString { get; set; }
	}
}

