using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFMPEGScreenRecorder
{
    class Program
    {
        public static bool quitFlag = false;
        public static string args = "";
        public static string c_args = "";
        public static string outputFileName= "";
        public static bool IsCompression = false;
        public static bool ThreadException = false;

        static void Main(string[] args)
        {
            try
            {
                LoadConfigFromINI();

                /*Screen Recording Thread*/
                ThreadStart setRecordingThread = new ThreadStart(() => StartRecording());
                Thread recordingThread = new Thread(setRecordingThread);
                recordingThread.Name = "Recording";
                recordingThread.Start();

                if (ThreadException)
                    throw new SystemException();

                /*User Input to Stop Recording*/
                bool stopFlag = false;
                ConsoleKey response;
                do
                {
                    Console.Write("Press Enter key to stop recording....\n");
                    response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show

                    if (response == ConsoleKey.Enter)
                    {
                        if (ThreadException)
                            throw new SystemException();
                        stopFlag = true;
                        quitFlag = true;
                        Console.WriteLine("Screen Recording is saved.");
                    }
                } while (stopFlag != true);

                /*Compression Thread*/
                if (IsCompression)
                {
                    ThreadStart setCompressionThread = new ThreadStart(() => StartCompression());
                    Thread compressionThread = new Thread(setCompressionThread);
                    compressionThread.Name = "Compression";

                    recordingThread.Join();
                    compressionThread.Start();
                    compressionThread.Join();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press Any key to exit.....");
                Console.ReadKey();
            }
        }

        public static void LoadConfigFromINI()
        {
            try
            {
                if (!File.Exists("config.ini"))
                {
                    //Create File
                    Console.WriteLine("Unable to locate config.ini. Creating Configuration file with default values");
                    CreateINIFile();
                }

                CreateArguments();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void StartRecording()
        {
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = args + " -y",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = false,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };
                process.Start();

                string processOutput = null;
                while ((processOutput = process.StandardError.ReadLine()) != null)
                {
                    if (quitFlag)
                    {
                        process.StandardInput.WriteLine("q");
                        process.WaitForExit();
                    }
                    Debug.WriteLine(processOutput);
                }
            }
            catch (SystemException)
            {
                Console.WriteLine("Error! \nFFMPEG Not found. Kindly Install FFMPEG from here : https://ffmpeg.org/download.html ");
                ThreadException = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Start Recording Thread Exception :- " + ex.Message);
            }
        }

        public static void StartCompression()
        {
            Console.WriteLine("Compressing Recorded Video....");
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = c_args + " -y",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };
                process.Start();

                string processOutput = null;
                while ((processOutput = process.StandardError.ReadLine()) != null)
                {
                    // do something with processOutput
                    Debug.WriteLine(processOutput);
                }
                Console.WriteLine("Video is compressed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Start Compression Thread Exception :- " + ex.Message);
                throw ex;
            }
        }

        public static void CreateINIFile()
        {
            try
            {
                var Ini = new IniFile();
                /*Recording Arguments*/
                Ini.Write("framerate", "30");
                Ini.Write("device", "gdigrab");
                Ini.Write("infile", "desktop");
                Ini.Write("outputFileName", "output.mkv");
                Ini.Write("compressVideo", "false");

                /*Compression Arguments config*/
                Ini.Write("videoCodec", "libx264");
                Ini.Write("compression-outputFileName", "compressed-ouptut.mkv");
                Ini.Write("constantRateFactor", "28");
            }
            catch(Exception ex)
            {
                Debug.WriteLine("CreateINIFile - " + ex.Message);
                throw;
            }
        }

        public static void CreateArguments()
        {
            try
            {
                // Creates or loads an INI file in the same directory as your executable
                // named EXE.ini (where EXE is the name of your executable)

                // Or specify a specific name in the current dir
                var Ini = new IniFile("config.ini");

                #region Framerate
                if (Ini.KeyExists("framerate"))
                {
                    if (Ini.Read("framerate") != null)
                        args += "-framerate " + Ini.Read("framerate") + " ";
                    else
                        throw new Exception("Invalid Framerate");
                }
                else
                {
                    Console.WriteLine("Using Default framerate:30");
                    Ini.Write("framerate", "30");
                    args += "-framerate " + Ini.Read("framerate") + " ";
                }
                #endregion

                #region Input Device
                if (Ini.KeyExists("device"))
                {
                    if (Ini.Read("device") != null)
                        args += "-f " + Ini.Read("device") + " ";
                    else
                        throw new Exception("Invalid Input Device");
                }
                else
                {
                    Console.WriteLine("Using Default input device: gdigrab");
                    Ini.Write("device", "gdigrab");
                    args += "-f " + Ini.Read("device") + " ";
                }
                #endregion

                #region Input File
                if (Ini.KeyExists("infile"))
                {
                    if (Ini.Read("infile") != null)
                        args += "-i " + Ini.Read("infile") + " ";
                    else
                        throw new Exception("Invalid Input file");
                }
                else
                {
                    Console.WriteLine("Using Default infile: Desktop");
                    Ini.Write("infile", "desktop");
                    args += "-i " + Ini.Read("infile") + " ";
                }
                #endregion

                #region Output File Name
                if (Ini.KeyExists("outputFileName"))
                {
                    if (Ini.Read("outputFileName") != null)
                    {
                        outputFileName = Ini.Read("outputFileName");
                        args += outputFileName + " ";
                    }

                    else
                        throw new Exception("Invalid Output File Name");
                }
                else
                {
                    Console.WriteLine("Using Default Output File Name: output.mkv");
                    Ini.Write("outputFileName", "output.mkv");
                    outputFileName = Ini.Read("outputFileName");
                    args += outputFileName + " ";
                }
                #endregion

                /*Compression Arguments*/

                c_args += "-i " + outputFileName + " ";

                #region Check IsCompression 
                if (Ini.KeyExists("compressVideo"))
                {
                    if (Ini.Read("compressVideo") != null)
                    {
                        IsCompression = (Ini.Read("compressVideo") == "true");
                    }
                    else
                        throw new Exception("Compress Video config is invalid");
                }
                else
                {
                    //Console.WriteLine("Using Default Output File Name: output.mkv");
                    Ini.Write("compressVideo", "false");
                    IsCompression = false;
                }
                #endregion

                #region Video Codec
                if (Ini.KeyExists("videoCodec"))
                {
                    if (Ini.Read("videoCodec") != null)
                        c_args += "-vcodec " + Ini.Read("videoCodec") + " ";
                    else
                        throw new Exception("Invalid Output File Name");
                }
                else
                {
                    Console.WriteLine("Using Default Video Codec:libx264");
                    Ini.Write("videoCodec", "libx264");
                    c_args += Ini.Read("videoCodec") + " ";
                }
                #endregion

                #region Constant Rate Factor
                if (Ini.KeyExists("constantRateFactor"))
                {
                    if (Ini.Read("constantRateFactor") != null)
                        c_args += "-crf " + Ini.Read("constantRateFactor") + " ";
                    else
                        throw new Exception("Invalid Constant Rate Factor");
                }
                else
                {
                    Console.WriteLine("Using Default Constant Rate Factor: 28");
                    Ini.Write("constantRateFactor", "28");
                    c_args += "-crf " + Ini.Read("constantRateFactor") + " ";
                }
                #endregion

                #region Output File Name
                if (Ini.KeyExists("compression-outputFileName"))
                {
                    if (Ini.Read("compression-outputFileName") != null)
                    {
                        c_args += Ini.Read("compression-outputFileName") + " ";
                    }

                    else
                        throw new Exception("Invalid Output File Name");
                }
                else
                {
                    Console.WriteLine("Using Default Output File Name: compressed-ouptut.mkv");
                    Ini.Write("compression-outputFileName", "compressed-ouptut.mkv");
                    c_args += Ini.Read("compression-outputFileName") + " ";
                }
                #endregion

            }catch(Exception ex)
            {
                Debug.WriteLine("CreateArguments - " + ex.Message);
                throw;
            }
        }
    }
}
