using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp7;
using AVT.VmbAPINET;
using AsynchronousGrabConsole;
using System.IO;


namespace Sharp7ZelfTest
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// 

        static void Main(string[] args)
        {
            string fileName = string.Empty;        // The FileName to store the image
            bool printHelp = false;                // Output help?
            string cameraID = null;                // The camera ID
            int tellerNaam = 0;



            while (true)
            {
                var clientLaminator = new S7Client();
                int connectionResult = clientLaminator.ConnectTo("10.0.0.151", 0, 1);

                if (connectionResult == 0)
                {
                    Console.WriteLine("con ok");
                }
                else
                {
                    Console.WriteLine("err");
                }

                //PARAMETERS
//              var Parameterbuffer = new byte[8];
//                int readRest = clientLaminator.DBRead(2, 0, Parameterbuffer.Length, Parameterbuffer);

//                if (readRest == 0)
//                {
//                    Console.WriteLine("dB2 ok");
//                }

//                double Temperatuur = S7.GetRealAt(Parameterbuffer, 0);
//                Console.WriteLine(Temperatuur.ToString());

//                int Druk = S7.GetIntAt(Parameterbuffer, 4);
//                Console.WriteLine(Druk.ToString());

//                int LamiSucces = S7.GetIntAt(Parameterbuffer, 6);
//                Console.WriteLine(LamiSucces.ToString());



                //FOTO
                var FotoBuffer = new byte[1];
                int readReste = clientLaminator.DBRead(2000, 0, FotoBuffer.Length, FotoBuffer);

                if (readReste == 0)
                {
                    Console.WriteLine("dB3 ok");
                }
                //S7.SetBitAt(ref FotoBuffer, Pos: 0, Bit: 0, Value: false);
                bool FotoNemen = S7.GetBitAt(FotoBuffer, 0, 0);
                Console.WriteLine(FotoNemen.ToString());
                if (FotoNemen == true)
                    {
                    // neem foto
                    //CHARS 
                    var CharSerienummerbuffer = new byte[10];
                    int readRes = clientLaminator.DBRead(1, 0, CharSerienummerbuffer.Length, CharSerienummerbuffer);

                    string Letter1;
                    Letter1 = S7.GetCharsAt(CharSerienummerbuffer, 0, 1);
                    //Console.WriteLine(Letter1);
                    string Letter2;
                    Letter2 = S7.GetCharsAt(CharSerienummerbuffer, 1, 1);
                    //Console.WriteLine(Letter2);
                    string Letter3;
                    Letter3 = S7.GetCharsAt(CharSerienummerbuffer, 2, 1);
                    //Console.WriteLine(Letter3);
                    string Letter4;
                    Letter4 = S7.GetCharsAt(CharSerienummerbuffer, 3, 1);
                    //Console.WriteLine(Letter4);
                    string Letter5;
                    Letter5 = S7.GetCharsAt(CharSerienummerbuffer, 4, 1);
                    //Console.WriteLine(Letter5);
                    string Letter6;
                    Letter6 = S7.GetCharsAt(CharSerienummerbuffer, 5, 1);
                    //Console.WriteLine(Letter6);
                    string Letter7;
                    Letter7 = S7.GetCharsAt(CharSerienummerbuffer, 6, 1);
                    //Console.WriteLine(Letter7);
                    string Letter8;
                    Letter8 = S7.GetCharsAt(CharSerienummerbuffer, 7, 1);
                    //Console.WriteLine(Letter8);
                    string Letter9;
                    Letter9 = S7.GetCharsAt(CharSerienummerbuffer, 8, 1);
                    //Console.WriteLine(Letter9);
                    string Letter10;
                    Letter10 = S7.GetCharsAt(CharSerienummerbuffer, 9, 1);
                    //Console.WriteLine(Letter10);


                    string Serienummer = Letter1 + Letter2 + Letter3 + Letter4 + Letter5 + Letter6 + Letter7 + Letter8 + Letter9 + Letter10;
                    //Console.WriteLine(Serienummer);
                    
                    
                try
                    {
                        ParseCommandLine(args, ref fileName, ref printHelp, ref cameraID);

                        if (fileName == string.Empty)
                        {
                            fileName = "SynchronousGrab.bmp";
                        }

                        // Print out help and end program
                        if (printHelp)
                        {
                          //  Console.WriteLine("Usage: AsynchronousGrab [CameraID] [/i] [/h]");
                          //  Console.WriteLine("Parameters:   CameraID    ID of the camera to use (using first camera if not specified)");
                          //  Console.WriteLine("              /f          FileName to save the image");
                          //  Console.WriteLine("              /h          Print out help");
                        }
                        else
                        {
                            // Create a new Vimba entry object
                            VimbaHelper vimbaHelper = new VimbaHelper();
                            vimbaHelper.Startup(); // Startup API
                                                   // Open camera
                            try
                            {
                                Console.WriteLine("Vimba .NET API Version {0}", vimbaHelper.GetVersion());
                                if (null == cameraID)
                                {
                                    // Open first available camera

                                    // Fetch all cameras known to Vimba
                                    List<Camera> cameras = vimbaHelper.CameraList;
                                    if (cameras.Count < 0)
                                    {
                                        throw new Exception("No camera available.");
                                    }

                                    foreach (Camera currentCamera in cameras)
                                    {
                                        // Check if we can open the camera in full mode
                                        VmbAccessModeType accessMode = currentCamera.PermittedAccess;
                                        if (VmbAccessModeType.VmbAccessModeFull == (VmbAccessModeType.VmbAccessModeFull & accessMode))
                                        {
                                            // Now get the camera ID
                                            cameraID = currentCamera.Id;
                                        }
                                    }

                                    if (null == cameraID)
                                    {
                                        throw new Exception("Could not open any camera.");
                                    }
                                }

                                Console.WriteLine("Opening camera with ID: " + cameraID);

                                System.Drawing.Image img = vimbaHelper.AcquireSingleImage(cameraID);

                                string lastDatetime = DateTime.Now.ToLongDateString();

                                if (File.Exists("C:\\Fotologs\\" + tellerNaam + lastDatetime + "voor laminatie" + ".bmp"))
                            {
                                img.Save("C:\\Fotologs\\" + tellerNaam + lastDatetime + "na laminatie" + ".bmp");
                                    tellerNaam++;
                            }
                            else
                            {
                                img.Save("C:\\Fotologs\\" + tellerNaam + lastDatetime + "voor laminatie" + ".bmp");
                            }
                                

                                Console.WriteLine("Image is saved as: " + Serienummer);
                            }
                            finally
                            {
                                // shutdown the vimba API
                                vimbaHelper.Shutdown();
                            }
                        }
                    }
                    catch (VimbaException ve)
                    {
                        // Output in case of a vimba Exception 
                        Console.WriteLine(ve.Message);
                        Console.Write("Return Code: " + ve.ReturnCode.ToString() + " (" + ve.MapReturnCodeToString() + ")");
                    }
                    catch (Exception e)
                    {
                        // Output in case of a System.Exception
                        Console.WriteLine(e.Message);
                    }


                    Console.WriteLine("foto genomen");
                    S7.GetBitAt(FotoBuffer, 0, 0);

                    Console.WriteLine("External: " + FotoBuffer[0]);

                    S7.SetBitAt(ref FotoBuffer, Pos: 0, Bit: 0,Value: false);

                    Console.WriteLine("Internal: " + FotoBuffer[0]);

                    clientLaminator.DBWrite(2000, 0, FotoBuffer.Length, FotoBuffer);

                }

                clientLaminator.Disconnect();
            }
                
            
           


        }


        /// <summary>
        /// Parses the Command Line Arguments
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// <param name="fileName">Holds the optional fileName</param>
        /// <param name="printHelp">Flag to decide if help information is shown</param>
        /// <param name="cameraID">The camera ID</param>
        private static void ParseCommandLine(string[] args, ref string fileName, ref bool printHelp, ref string cameraID)
        {
            // Parse command line
            foreach (string parameter in args)
            {
                if (parameter.Length < 0)
                {
                    throw new ArgumentException("Invalid parameter found.");
                }

                if (parameter.StartsWith("/"))
                {
                    if (parameter.StartsWith("/f:"))
                    {
                        if (string.Empty != fileName)
                        {
                            throw new ArgumentException("Invalid parameter found.");
                        }

                        fileName = parameter.Substring(3);
                        if (fileName.Length <= 0)
                        {
                            throw new ArgumentException("Invalid parameter found.");
                        }
                    }
                    else
                        if (string.Compare(parameter, "/h", StringComparison.Ordinal) == 0)
                    {
                        if (true == printHelp)
                        {
                            throw new ArgumentException("Invalid parameter found.");
                        }

                        printHelp = true;
                    }
                }
                else
                {
                    if (null != cameraID)
                    {
                        throw new ArgumentException("Invalid parameter found.");
                       
                    }

                    cameraID = parameter;
                }
            }
        }


        

    }
}
