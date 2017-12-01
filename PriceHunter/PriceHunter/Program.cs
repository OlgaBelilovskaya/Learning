using System;
using System.Collections.Generic;
using System.Configuration;
using HtmlAgilityPack;
using Unity;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace PriceHunter
{
	public interface IParcer
	{
		string Parce(string url, string xpath);
	}

	public class Parcer : IParcer
	{
		public string Parce(string url, string xpath)
		{
			//var webGet = new HtmlWeb();
			//var page = webGet.Load("http://www2.hm.com/ru_ru/sale/shopbyproductkids/girls-size18m-10y.html");

			var page = new HtmlDocument();
			page.Load("d:\\test.html");

			//var tmp = "//*[contains(text(),'Трегинсы')]"; //!!!!! Работает
			//var tmp = "//*[contains(text(),'Трегинсы')]/parent::h3[@class='product-item-heading']"; //!!!Работает

			var tmp = "((//strong[(contains(concat(' ', normalize-space(@class), ' '),' sale-price')) and ../../h3/a[contains(text(),'Трегинсы')]])//text())[1]";

			var node = page.DocumentNode.SelectNodes(tmp);

			string text = node[0].InnerText;
			var numericPart = Regex.Match(text, "\\d+").Value;

			return numericPart;
		}
	}

	public interface ICongigData
	{
		string GetSectionData(string tag);
	}
		
	public class ConfigData : ICongigData
	{
		public string GetSectionData(string tag)
		{
			return ConfigurationManager.AppSettings[tag];
		}
	}

	public interface IDecisionMaker
	{
		bool ShouldWeDoSmng(string oldValue, string newValue);
	}

	public class PriceWentDown : IDecisionMaker
	{
		public bool ShouldWeDoSmng(string oldValue, string newValue)
		{
			return (Convert.ToInt32(oldValue) > Convert.ToInt32(newValue));
		}
	}

	public interface ISaver
	{
		string Read();
		void Write(string value);
	}

	public class FileSaver : ISaver
	{
		private string fileName = "D:\\price.txt";

		public string Read()
		{
			return File.ReadAllText(fileName);
		}

		public void Write(string value)
		{
			File.WriteAllText(fileName, value);
		}
	}

	public interface INotifier
	{
		void Notify(string value);
	}

	public class MailNotifier : INotifier
	{
		public void Notify(string value)
		{
			MailMessage mail = new MailMessage("olga.belilovskaya@bk.ru", "olga.belilovskaya@gmail.com");
			SmtpClient client = new SmtpClient();
			client.Port = 587;
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = false;
			client.Host = "smtp.mail.ru";
			client.EnableSsl = true;
			client.Credentials = new System.Net.NetworkCredential("olga.belilovskaya@bk.ru", "hiitsme!b");
			mail.Subject = "this is a test email.";
			mail.Body = $"Look! The price id down to {value} - don't you wanna miss it, babe?";
			client.Send(mail);
		}	
	}

	public class PriceHunter
	{
		private ICongigData _configData;
		private IParcer _parcer;
		private ISaver _saver;
		private INotifier _notifier;
		private IDecisionMaker _decisionMaker;
		
		public PriceHunter(ICongigData configData, IParcer parcer, ISaver saver, INotifier notifier, IDecisionMaker decisionMaker)
		{
			_configData = configData;
			_parcer = parcer;
			_saver = saver;
			_notifier = notifier;
			_decisionMaker = decisionMaker;
		}

		public void Start()
		{
			var url = _configData.GetSectionData("url");
			var xpath = _configData.GetSectionData("xpath");
			string price = _parcer.Parce(url, xpath);
			//Вытащить из файла старую цену и сохранить туда новую
			string oldPrice = _saver.Read();
			//Вызвать сравниватель цен
			if(_decisionMaker.ShouldWeDoSmng(oldPrice, price))			
			{
				_saver.Write(price);
				_notifier.Notify(price);
			}
		}
	}

	class Program
	{
		static void Main(string[] args)
		{			
			IUnityContainer container = new UnityContainer();

			container.RegisterType<ICongigData, ConfigData>();
			container.RegisterType<IParcer, Parcer>();
			container.RegisterType<ISaver, FileSaver>();
			container.RegisterType<INotifier, MailNotifier>();
			container.RegisterType<IDecisionMaker, PriceWentDown>();

			PriceHunter priceHunter = container.Resolve<PriceHunter>();

			priceHunter.Start();
		}
	}
}
