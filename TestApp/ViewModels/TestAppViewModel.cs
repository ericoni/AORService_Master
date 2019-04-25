using ActiveAORCache.Helpers;
using AORViewer.Model;
using AORViewer.ViewModels;
using EventAlarmProxyNS;
using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TestApp.ViewModels
{
	public class TestAppViewModel : ViewModelBase
	{
		private List<LBModelBase> aorViewerList;
		private List<LBModelBase> aorViewerTempList;
		//private EventSubscriberCallback eventSubscriberCallback;

		public TestAppViewModel()
		{
			//EventSubscriberCallback eventSubCallback = new EventSubscriberCallback();
			//EventSubscriberProxy eventSubProxy = new EventSubscriberProxy(eventSubCallback);
			//eventSubProxy.SubscribeToAORAreas(new HashSet<long>());

			/*	EventSubscriberCallback eventSubscriberCallback = EventSubscriberCallback.Instance;
				int counter = 0;

				while (true)
				{
					Thread.Sleep(3000);

					try
					{
						eventSubscriberCallback.ConnectToTest();
					}
					catch (Exception)
					{
						if(counter++ == 5)
							throw;
					} 
				} */

			aorViewerTempList = new List<LBModelBase>(4)  /// TODO odkomentarisi
			{ new LBModelBase(LBType.Permissions.ToString(), "Neki opis", @"..\..\..\Images\moreAM.jpg"),
				new LBModelBase(LBType.AOR_Groups.ToString(), "AOR GRUPE",@"..\..\..\Images\moreAM.jpg"),
				new LBModelBase(LBType.AOR_Areas.ToString(), "ARea",@"..\..\..\Images\moreAM.jpg"),
				new LBModelBase(LBType.DNA_Authorities.ToString(), "Dna nesto", @"..\..\..\Images\moreAM.jpg")};
			AORViewerList = aorViewerTempList;
		}

		public List<LBModelBase> AORViewerList
		{
			get
			{
				return aorViewerList;
			}

			set
			{
				aorViewerList = value;
				OnPropertyChanged("AORViewerList");
			}
		}

		public void Refresh()
		{
			MessageBox.Show("Refresh received.");
		}

		//public string DisplayedImage
		//{
		//    get { return @"C:\Users\Dragan\Pictures\DebaSomiJa.jpg"; }
		//    //C:\Users\Dragan\Pictures
		//    //C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.jpg
		//}
	}
}
