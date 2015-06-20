using System;
using System.Drawing;
using System.Windows.Forms;

class MyForm : Form
{
    public MyForm()
    {
        InitializeComponent();

        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        AllowDrop = true; // Because we want to be a drop target of windows explorer files.

        // TestForm will containt the per-pixel-alpha dib
        TestForm = new MyPerPixelAlphaForm();
        TestForm.Show();
    }


    ///<para>Constructs and initializes all child controls of this dialog box.</para>
    private void InitializeComponent()
    {
        ClientSize = new Size(350, 160);
        Font = new Font("tahoma", 8);
        Text = "perpixelalpha# - Sample application";

        // Label with to display current opacity level
        Label Label1 = new Label();
        Label1.AutoSize = true;
        Label1.Location = new System.Drawing.Point(4, 8);
        Label1.Text = "1. Drag&&Drop an image file from windows explorer into this window.";
        Controls.Add(Label1);

        Label Label2 = new Label();
        Label2.AutoSize = true;
        Label2.Location = new System.Drawing.Point(4, 38);
        Label2.Text = "2. Play with the opacity level [0..255]:";
        Controls.Add(Label2);

        // Label with to display current opacity level
        LabelValue = new Label();
        LabelValue.AutoSize = true;
        LabelValue.Location = new System.Drawing.Point(195, 38);
        LabelValue.Text = "255";

        Controls.Add(LabelValue);


        // Trackbar to change opacity level
        Track = new TrackBar();

        Track.Location = new System.Drawing.Point(18, 58);
        Track.Size = new System.Drawing.Size(310, 0);
        Track.BeginInit();
        Track.Maximum = 255;
        Track.TickFrequency = 5;
        Track.TickStyle = TickStyle.TopLeft;
        Track.Value = 255;
        Track.EndInit();

        Track.ValueChanged += new EventHandler(Track_ValueChanged);

        Controls.Add(Track);


        Label Label3 = new Label();
        Label3.AutoSize = true;
        Label3.Location = new System.Drawing.Point(4, 108);
        Label3.Text = "3. Drag the layered window arround you desktop!";
        Controls.Add(Label3);


        // Label with two links to me! :)
        LinkLabel Link = new LinkLabel();

        Link.Location = new System.Drawing.Point(4, 140);
        Link.Size = new System.Drawing.Size(250, 80);
        Link.Text = "by Rui Lopes <rui@ruilopes.com>";
        Link.Links.Add(3, 9, "http://www.ruilopes.com");
        Link.Links.Add(14, 16, "mailto:rui@ruilopes.com");

        Link.LinkClicked += new LinkLabelLinkClickedEventHandler(Link_LinkClicked);

        Controls.Add(Link);
    }


    ///<para>Frees our bitmap.</para>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }


    ///<para>Accepts only Drops of windows explorer files.</para>
    protected override void OnDragEnter(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
        base.OnDragEnter(e);
    }


    ///<para>Just loads the dropped file from windows explorer.</para>
    protected override void OnDragDrop(DragEventArgs e)
    {
        string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (files != null)
        {
            if (files.Length == 1)
                SetPerPixelBitmapFilename(files[0]);
            else
                MessageBox.Show(this, "Please, drop only one image file.", "Too many files dropped", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
        base.OnDragDrop(e);
    }


    ///<para>Just load a image file and display it on our test form.</para>
    private void SetPerPixelBitmapFilename(string fileName)
    {
        Bitmap newBitmap;

        try
        {

            newBitmap = Image.FromFile(fileName) as Bitmap;
            TestForm.SetBitmap(newBitmap, (byte)Track.Value);

        }
        catch (ApplicationException e)
        {
            MessageBox.Show(this, e.Message, "Error with bitmap.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        catch (Exception e)
        {
            MessageBox.Show(this, e.Message, "Could not open image file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (bitmap != null)
            bitmap.Dispose();
        bitmap = newBitmap;
    }


    ///<para>Change the opacity level of our test form.</para>
    private void Track_ValueChanged(object sender, EventArgs e)
    {
        byte opacity = (byte)Track.Value;
        LabelValue.Text = opacity.ToString();
        LabelValue.Refresh(); // We need this because on slow computers (mine!) the windows takes some time to update our label.

        if (bitmap != null)
            TestForm.SetBitmap(bitmap, opacity);
    }


    ///<para>Start the computer browser in the specified uri.</para>
    private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        e.Link.Visited = true;
        using (System.Diagnostics.Process.Start(e.Link.LinkData.ToString()))
        {
        }
    }


    private Label LabelValue;	// label with current opacity level
    private TrackBar Track;		// trackbar to chabge opacity level
    private MyPerPixelAlphaForm TestForm;	// our test form
    private Bitmap bitmap;		// bitmap that is currently displaying on our test form
}

class MyPerPixelAlphaForm : PerPixelAlphaForm
{
    public MyPerPixelAlphaForm()
    {
        TopMost = true;
        ShowInTaskbar = false;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0084)
        {
            m.Result = (IntPtr)2;
            return;
        }
        base.WndProc(ref m);
    }
}
