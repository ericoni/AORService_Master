using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AORViewer.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private bool disposed = false;

		/// <summary>
		/// Gets or sets the user-friendly name of this object.
		/// Child classes can set this property to a new value,
		/// or override it to determine the value on-demand.
		/// </summary>
		public virtual string DisplayName { get; protected set; }

		/// <summary>
		/// Gets a value indicating whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

		public virtual void Dispose()
		{
			CleanUp(true);
			GC.SuppressFinalize(this);
		}

		private void CleanUp(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// release managed resources
					OnDispose();
				}

				// release unmanaged resources
				OnDisposeUnmanaged();
			}

			disposed = true;
		}

		public void FirePropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <typeparam name="T">Type of the property</typeparam>
		/// <param name="storage">Current property value.</param>
		/// <param name="value">The new value.</param>
		/// <param name="propertyName">Name of the property.</param>
		protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (!Equals(storage, value))
			{
				storage = value;
				OnPropertyChanged(propertyName);
			}
		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.VerifyPropertyName(propertyName);

			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void OnPropertyChanged(params string[] propertyNames)
		{
			foreach (string propertyName in propertyNames)
			{
				OnPropertyChanged(propertyName);
			}
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// Verify that the property name matches a real,
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;

				if (this.ThrowOnInvalidPropertyName)
				{
					throw new Exception(msg);
				}
				else
				{
					Debug.Fail(msg);
				}
			}
		}

		/// <summary>
		/// Child classes can override this method to perform
		/// clean-up logic, such as removing event handlers.
		/// </summary>
		protected virtual void OnDispose()
		{
		}

		/// <summary>
		/// Override this method if there are any unmanaged resources left to clean up
		/// </summary>
		protected virtual void OnDisposeUnmanaged()
		{
		}
	}
}
