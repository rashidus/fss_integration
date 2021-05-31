using System.ServiceProcess;

namespace FSSService
{
    partial class FSSWindowsServiceInstaller
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // serviceInstaller1
            //
#if (DEBUG)
            this.serviceInstaller1.ServiceName = "FSSWindowsService_DEBUG";
            this.serviceInstaller1.DisplayName = "ФСС. Служба для обмена данными с ФСС (отладочная)";
            this.serviceInstaller1.Description = "Служба создана для отладки логики подключения и обмена данными с порталом ФСС.";
#else
            this.serviceInstaller1.ServiceName = "FSSWindowsService";
            this.serviceInstaller1.DisplayName = "ФСС. Служба для обмена данными с ФСС";
            this.serviceInstaller1.Description = "Служба содержит в себе логику подключения и обмена данными с порталом ФСС";
#endif
            // 
            // ProjectInstaller
            // 
            this.serviceInstaller1.StartType = ServiceStartMode.Automatic;
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});

        }

#endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}