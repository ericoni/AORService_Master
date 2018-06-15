using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Xml;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace DERMSApp.ViewModels
{
    public class DeltaViewModel: ViewModelBase
    {
        private CIMAdapter adapter = new CIMAdapter();
        private Delta nmsDelta = null;
        private Visibility reportVisible;

        public DeltaViewModel()
        {
            InitGUIElements();
            ButtonBrowseLocationOnClick = new RelayCommand(() => ButtonBrowseLocationOnClickExecute(), () => true);
            ButtonConvertCIMOnClick = new RelayCommand(() => ButtonConvertCIMOnClickExecute(), () => true);
            ButtonApplyDeltaOnClick = new RelayCommand(() => ButtonApplyDeltaOnClickExecute(), () => true);
            ButtonExitOnClick = new RelayCommand(() => ButtonExitOnClickExecute(), () => true);
        }

       // public ICommand InitGUIElements { get; private set; }
        public ICommand ButtonBrowseLocationOnClick { get; private set; }
        public ICommand ButtonConvertCIMOnClick { get; private set; }
        public ICommand ButtonApplyDeltaOnClick { get; private set; }
        public ICommand ButtonExitOnClick { get; private set; }

        private void InitGUIElements()
        {
            TextBoxCIMFileText = "";
            ButtonConvertCIMEnabled = false;
            ButtonApplyDeltaEnabled = false;
            RichTextBoxReportText = "";
            ReportVisible = Visibility.Hidden;

            ComboBoxProfileDataSource = Enum.GetValues(typeof(SupportedProfiles));
            ComboBoxProfileSelectedItem = SupportedProfiles.DERMS;
            //comboBoxProfile.Enabled = false; //// other profiles are not supported

        }
        private void ButtonBrowseLocationOnClickExecute()
        {
            ShowOpenCIMXMLFileDialog();
        }

        private void ButtonConvertCIMOnClickExecute()
        {
            ConvertCIMXMLToDMSNetworkModelDelta();
            ReportVisible = Visibility.Visible;
        }

        private void ButtonApplyDeltaOnClickExecute()
        {
            ApplyDMSNetworkModelDelta();
        }

        private void ButtonExitOnClickExecute()
        {
            //System.Windows.Application.Current.MainWindow.Close();
        }

        private void ShowOpenCIMXMLFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open CIM Document File..";
            openFileDialog.Filter = "CIM-XML Files|*.xml;*.txt;*.rdf|All Files|*.*";
            openFileDialog.RestoreDirectory = true;

            DialogResult dialogResponse = openFileDialog.ShowDialog();
            if (dialogResponse == DialogResult.OK)
            {
                TextBoxCIMFileText = openFileDialog.FileName;
                //toolTipControl.SetToolTip(textBoxCIMFile, openFileDialog.FileName);
                ButtonConvertCIMEnabled = true;
                RichTextBoxReportText = "";
            }
            else
            {
                ButtonConvertCIMEnabled = false;
            }
        }

        private void ConvertCIMXMLToDMSNetworkModelDelta()
        {
            ////SEND CIM/XML to ADAPTER
            try
            {
                if (TextBoxCIMFileText == string.Empty)
                {
                    System.Windows.Forms.MessageBox.Show("Must enter CIM/XML file.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string log;
                nmsDelta = null;
                using (FileStream fs = File.Open(TextBoxCIMFileText, FileMode.Open))
                {
                    nmsDelta = adapter.CreateDelta(fs, (SupportedProfiles)(ComboBoxProfileSelectedItem), out log);
                    RichTextBoxReportText = log;
                }
                if (nmsDelta != null)
                {
                    //// export delta to file
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(".\\deltaExport.xml", Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        nmsDelta.ExportToXml(xmlWriter);
                        xmlWriter.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("An error occurred.\n\n{0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ButtonApplyDeltaEnabled = (nmsDelta != null);
            TextBoxCIMFileText = string.Empty;
        }

        private void ApplyDMSNetworkModelDelta()
        {
            //// APPLY Delta
            if (nmsDelta != null)
            {
                try
                {
                    string log = adapter.ApplyUpdates(nmsDelta);
                    RichTextBoxReportText += log;
                    nmsDelta = null;
                    ButtonApplyDeltaEnabled = (nmsDelta != null);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("An error occurred.\n\n{0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No data is imported into delta object.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        bool _buttonConvertCIMEnabled;
        bool _buttonApplyDeltaEnabled;
        SupportedProfiles _comboBoxProfileSelectedItem;
        string _textBoxCIMFileText;
        string _richTextBoxReportText;
        Array _comboBoxProfileDataSource;

        public bool ButtonConvertCIMEnabled
        {
            get
            {
                return _buttonConvertCIMEnabled;
            }
            set
            {
                if (_buttonConvertCIMEnabled == value)
                    return;
                _buttonConvertCIMEnabled = value;
                RaisePropertyChanged("ButtonConvertCIMEnabled");
            }
        }
        public bool ButtonApplyDeltaEnabled
        {
            get
            {
                return _buttonApplyDeltaEnabled;
            }
            set
            {
                if (_buttonApplyDeltaEnabled == value)
                    return;
                _buttonApplyDeltaEnabled = value;
                RaisePropertyChanged("ButtonApplyDeltaEnabled");
            }
        }

        public SupportedProfiles ComboBoxProfileSelectedItem
        {
            get
            {
                return _comboBoxProfileSelectedItem;
            }
            set
            {
                if (_comboBoxProfileSelectedItem == value)
                    return;
                _comboBoxProfileSelectedItem = value;
                RaisePropertyChanged("ComboBoxProfileSelectedItem");
            }
        }
        public string TextBoxCIMFileText
        {
            get
            {
                return _textBoxCIMFileText;
            }
            set
            {
                if (_textBoxCIMFileText == value)
                    return;
                _textBoxCIMFileText = value;
                RaisePropertyChanged("TextBoxCIMFileText");
            }
        }

        public string RichTextBoxReportText
        {
            get
            {
                return _richTextBoxReportText;
            }
            set
            {
                if (_richTextBoxReportText == value)
                    return;
                _richTextBoxReportText = value;
                RaisePropertyChanged("RichTextBoxReportText");
            }
        }

        public Array ComboBoxProfileDataSource
        {
            get
            {
                return _comboBoxProfileDataSource;
            }
            set
            {
                if (_comboBoxProfileDataSource == value)
                    return;
                _comboBoxProfileDataSource = value;
                RaisePropertyChanged("ComboBoxProfileDataSource");
            }
        }

        public Visibility ReportVisible
        {
            get
            {
                return reportVisible;
            }

            set
            {
                if (reportVisible == value)
                    return;
                reportVisible = value;
                RaisePropertyChanged("ReportVisible");
            }
        }
    }
}
