using DERMSApp.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DERMSApp.ViewModels
{
    public class DialogBoxViewModel : BindableBase
    {
        public event EventHandler Close;
        public event EventHandler DoSomething;

        private string messageToShow;
        private string messageIcon;
        private string caption;
        private Visibility okButtonVisible;
        private Visibility yesNoButtonsVisible;

        public ICommand CloseCommand { get; private set; }
        public ICommand DoSomethingCommand { get; private set; }

        public DialogBoxViewModel()
        {
            this.CloseCommand = new RelayCommand(() => this.Close(this, new EventArgs()));
            this.DoSomethingCommand = new RelayCommand(() => this.DoSomething(this, new EventArgs()));
        }

        public DialogBoxViewModel(string caption, bool showOkButton, string message, int icon)
        {
            if(showOkButton)
            {
                OkButtonVisible = Visibility.Visible;
                YesNoButtonsVisible = Visibility.Collapsed;
            }
            else
            {
                OkButtonVisible = Visibility.Collapsed;
                YesNoButtonsVisible = Visibility.Visible;
            }
            Caption = caption;
            MessageToShow = "\n"+message;
            if(icon==1)
            {
                MessageIcon = @"../Images/InfoIcon.png";
            }
            else if(icon==2)
            {
                MessageIcon = @"../Images/WarningIcon.png";
            }
            else
            {
                MessageIcon = @"../Images/ErrorIcon.png";
            }
            this.CloseCommand = new RelayCommand(() => this.Close(this, new EventArgs()));
            this.DoSomethingCommand = new RelayCommand(() => this.DoSomething(this, new EventArgs()));
        }

        public string MessageToShow
        {
            get
            {
                return messageToShow;
            }

            set
            {
                messageToShow = value;
                OnPropertyChanged("MessageToShow");
            }
        }

        public Visibility OkButtonVisible
        {
            get
            {
                return okButtonVisible;
            }

            set
            {
                okButtonVisible = value;
                OnPropertyChanged("OkButtonVisible");
            }
        }

        public Visibility YesNoButtonsVisible
        {
            get
            {
                return yesNoButtonsVisible;
            }

            set
            {
                yesNoButtonsVisible = value;
                OnPropertyChanged("YesNoButtonsVisible");
            }
        }

        public string Caption
        {
            get
            {
                return caption;
            }

            set
            {
                caption = value;
                OnPropertyChanged("Caption");
            }
        }

        public string MessageIcon
        {
            get
            {
                return messageIcon;
            }

            set
            {
                messageIcon = value;
                OnPropertyChanged("MessageIcon");
            }
        }
    }
}
