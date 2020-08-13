using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Csla;
using Csla.Xaml;
using Machine.Specifications;

namespace ViewModelBaseIsBusyBug.Test {
	[Subject(typeof(ViewModelBase<>))]
	public class ViewModelBaseSpec {
		private static int isBusyCount;
		private static ViewModel SUT { get; set; }

		Establish context = () => {
			isBusyCount = 0;

			SUT = new ViewModel();
			SUT.PropertyChanged += VM_PropertyChanged;
		};

		Cleanup cleanup = () => SUT.PropertyChanged -= VM_PropertyChanged;

		Because of = () => SUT.SaveItAsync().Await();

		It should_raise_is_busy_twice = () => isBusyCount.ShouldEqual(2);

		private static void VM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == nameof(ViewModel.IsBusy)) {
				isBusyCount += 1;
			}
		}

		[Serializable]
		private sealed class BusinessObject : BusinessBase<BusinessObject> {
			public static BusinessObject Create() => DataPortal.Create<BusinessObject>();
		}

		private sealed class ViewModel : ViewModelBase<BusinessObject> {
			public ViewModel() => Model = BusinessObject.Create();

			public Task SaveItAsync() => SaveAsync();
		}
	}
}
