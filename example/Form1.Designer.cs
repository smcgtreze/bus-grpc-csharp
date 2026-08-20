namespace MapClickDemo
{
    partial class Form1
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;

        private void InitializeComponent()
        {
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.SuspendLayout();

            this.webView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView2.Location = new System.Drawing.Point(0, 0);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(800, 450);
            this.webView2.TabIndex = 0;

            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webView2);
            this.Name = "Form1";
            this.Text = "Map Click Demo";

            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
