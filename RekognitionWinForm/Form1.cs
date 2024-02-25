using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace RekognitionWinForm
{
    public partial class Form1 : Form
    {
       public List<BoundingBox> values = new List<BoundingBox>();
       public List<TextDetection> textDetections = new List<TextDetection>();
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "images files (*.png)|*.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    Upload(filePath);


                }
            }
            pictureBox1.Image = System.Drawing.Image.FromFile(filePath);
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }

        private void Upload(string str)
        {
            string bucketName = "pavelbucket0307";
            string keyName = "neon.png";
            string filePath = str;

            // Set up your AWS credentials
            BasicAWSCredentials credentials = new BasicAWSCredentials("AKIA6ODU52MBLXV33VPH", "rAOOh5sDYlhfKXXHV9WVECCyuPN7z7io2wgIPbN3");

            // Create a new Amazon S3 client
            AmazonS3Client s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest1);

            try
            {
                // Upload the file to Amazon S3
                TransferUtility fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.Upload(filePath, bucketName, keyName);
                Console.WriteLine("Upload completed!");
                Rekognition();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }

        private async Task Rekognition()
        {

            String photo = "neon.png";
            String bucket = "pavelbucket0307";

            BasicAWSCredentials credentials = new BasicAWSCredentials("AKIA6ODU52MBLXV33VPH", "rAOOh5sDYlhfKXXHV9WVECCyuPN7z7io2wgIPbN3");
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(credentials, Amazon.RegionEndpoint.EUWest1);

            DetectTextRequest detectTextRequest = new DetectTextRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Name = photo,
                        Bucket = bucket
                    }
                }
            };

            try
            {
                //DetectTextResponse detectTextResponse = await rekognitionClient.DetectTextAsync(detectTextRequest);
                //Console.WriteLine("Detected lines and words for " + photo);

                //foreach (TextDetection text in detectTextResponse.TextDetections)
                //{

                //    values.Add(text.Geometry.BoundingBox);

                //}

                DetectTextResponse detectTextResponse = await rekognitionClient.DetectTextAsync(detectTextRequest);
                textDetections = detectTextResponse.TextDetections;
                Bitmap bmp = new Bitmap(pictureBox1.Image);

                using (Graphics g = Graphics.FromImage(bmp))
                using (Pen pen = new Pen(Color.Red, 2))
                using (Pen pen2 = new Pen(Color.Green, 2)) // Use a red pen with thickness 2
                {
                    foreach (TextDetection text in textDetections)

                    {

                        BoundingBox box = text.Geometry.BoundingBox;

                        int left = (int)(box.Left * bmp.Width);
                        int top = (int)(box.Top * bmp.Height);
                        int width = (int)(box.Width * bmp.Width);
                        int height = (int)(box.Height * bmp.Height);

                        Rectangle rect = new Rectangle(left, top, width, height);

                        //if (text.Type == "LINE")
                        //g.DrawRectangle(pen, rect);
                        //else
                        //g.DrawRectangle(pen2, rect);



                    }
                }

                pictureBox1.Image = bmp;



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text.ToLower();

            Bitmap bmp = new Bitmap(pictureBox1.Image);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen2 = new Pen(Color.Green, 2)) 
            {
                foreach (TextDetection text in textDetections)

                {

                    BoundingBox box = text.Geometry.BoundingBox;

                    int left = (int)(box.Left * bmp.Width);
                    int top = (int)(box.Top * bmp.Height);
                    int width = (int)(box.Width * bmp.Width);
                    int height = (int)(box.Height * bmp.Height);

                    Rectangle rect = new Rectangle(left, top, width, height);


                    if (text.DetectedText.ToLower() == str)
                        g.DrawRectangle(pen2, rect);
                    



                }
            }
            pictureBox1.Image = bmp;

        }
    } 
}
