namespace AvaloniaSample.ViewModels
{
    public class WarningDialogViewModel : BindableBase, IDialogAware
    {
        // 实现IDialogAware
        public string Title => "警告";// 弹出窗口的标题
        public DialogCloseListener RequestClose { get; }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        // 命令定义
        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public WarningDialogViewModel()
        {
            ConfirmCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));
            CancelCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        // 当弹出窗口关闭时执行的逻辑
        public void OnDialogClosed()
        {

        }

        // 当弹出窗口打开的时候执行的逻辑
        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 接收参数
            Message = parameters.GetValue<string>("message");
        }

        public bool CanCloseDialog() => true; // 允许直接关闭
    }
}
