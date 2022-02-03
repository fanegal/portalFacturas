using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ToPfx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }

        private void TextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void btncertificado_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de certificado(*.cer)|*.cer";
            if (openFileDialog.ShowDialog() == true)
                txtcertificado.Text = openFileDialog.FileName;  //    File.ReadAllText(openFileDialog.FileName);
        }

        private void btnKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos KEY(*.key)|*.key";
            if (openFileDialog.ShowDialog() == true)
                txtkey.Text = openFileDialog.FileName;
        }

        private void btnDestino_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                txtdestino.Text = dialog.FileName;
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtcertificado.Text))
            { MessageBox.Show("No ha seleccionado un archivo .cer"); }
            else
            {
                if (string.IsNullOrEmpty(txtkey.Text))
                { MessageBox.Show("No ha seleccionado un archivo .key"); }
                else
                {
                    if (string.IsNullOrEmpty(txtpassword.Password))
                    { MessageBox.Show("Falta la contraseña"); }
                    else
                    {
                        if (string.IsNullOrEmpty(txtdestino.Text))
                        { MessageBox.Show("Seleccione una carpeta de destino"); }
                        else
                        {
                            PFX oPFX = new PFX(txtcertificado.Text, txtkey.Text, txtpassword.Password, txtdestino.Text + "\\" + Guid.NewGuid().ToString() + ".pfx");

                            if (oPFX.creaPFX())
                            {
                                MessageBox.Show("Archivo creado con éxito");
                                txtlistresult.Items.Add(oPFX.detalle);
                                // Displays the MessageBox.
                                MessageBoxResult result = MessageBox.Show("¿Mostrar en carpeta?", "Open folder", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    Process proc = new Process();
                                    proc.EnableRaisingEvents = false;
                                    proc.StartInfo = new ProcessStartInfo("cmd", "/c " + " explorer.exe help /select," + txtdestino.Text + "\\");
                                    proc.Start();
                                }
                            }
                            else
                            {
                                txtlistresult.Items.Add(oPFX.detalle);
                                MessageBox.Show(oPFX.error);

                            }
                        }
                    }
                }
            }
        }
    }
}
