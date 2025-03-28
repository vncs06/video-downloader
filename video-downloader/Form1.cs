using System.Diagnostics;
using System.IO;

namespace video_downloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Por favor, insira um link válido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string outputPath = Path.Combine(folderDialog.SelectedPath, "%(title)s.%(ext)s");

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = @"C:\Users\Gabriel\OneDrive\Documentos\GitHub\video-downloader\video-downloader\api\yt-dlp",
                    Arguments = $" -f 270+140 --merge-output-format mp4 --progress --newline --no-simulate \"{url}\" -o \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = psi };
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        AtualizarProgresso(e.Data);
                        Console.WriteLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine("Erro: " + e.Data); 
                    }
                };

                try
                {
                    
                    await Task.Run(() =>
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    });

                    
                    MessageBox.Show("Download concluído!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao iniciar o download: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void AtualizarProgresso(string linha)
        {
            Console.WriteLine(linha); 

            if (linha.Contains("[download]"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(linha, @"(\d+(\.\d+)?)%");
                if (match.Success)
                {
                    double progresso = double.Parse(match.Groups[1].Value);
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = Math.Min(100, (int)progresso)));
                }
            }
        }
    }
}
