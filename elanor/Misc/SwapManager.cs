// DEPRECATED DUE TO USELESS AND LACK OF LOGIC AND ANY SENSE
using System;
using System.IO;
using System.Threading.Tasks;
using Elanor.Properties;
using Elanor.DatFile;
using shared;

namespace Elanor.Misc
{
    public class SwapManager
    {
        public SwapManager(string pathA, string pathB = null)
        {
            if (!string.IsNullOrWhiteSpace(pathA))
            {
                EntityA = new SwapEntity(pathA);
                //SetReserveEntity(pathB);
            }
        }

        public SwapEntity EntityA { get; private set; }

        // public SwapEntity EntityB { get; private set; }
        /*
         public void SetReserveEntity(string path)
         {
             if (!string.IsNullOrWhiteSpace(path) && string.CompareOrdinal(path, EntityA.DatFilePath) != 0)
             {
                 EntityB = new SwapEntity(path);
             }
         }

         public void RemoveReserveEntity()
         {
             if (EntityB != null)
             {
                 EntityB.CloseController();
                 EntityB = null;
             }
         }

         public async Task SwapAsync()
         {
             await Task.Run(Swap);
         }

         private async Task Swap()
         {
             if (!EntityA.IsValid || !EntityB.IsValid)
             {
                 Logger.Write("попытка использования неинициализированной локализации.");
                 return;
             }

             EntityA.DatController.Close();
             EntityB.DatController.Close();

             try
             {
                 var sourceDir = Path.GetDirectoryName(EntityA.DatFilePath);         
                 if (sourceDir == null)
                 {
                     throw new ArgumentException($"неисправный путь к из файлу локализации {EntityA.DatFilePath}.");
                 }

                 var tempName = Path.Combine(sourceDir, Path.GetRandomFileName());
                 var pathA = EntityA.DatFilePath;
                 var pathB = EntityB.DatFilePath;

                 await Task.Run(() => File.Move(pathA, tempName));
                 await Task.Run(() => File.Move(pathB, pathA));
                 await Task.Run(() => File.Move(tempName, pathB));
                 //await MoveFile(Orig.DatFile, tempName);
                 //await MoveFile(Alt.DatFile, Orig.DatFile);
                 //await MoveFile(tempName, Alt.DatFile);

                 EntityA = new SwapEntity(pathA);
                 EntityB = new SwapEntity(pathB);

                 await EntityA.InitAsync();
                 await EntityB.InitAsync();
             }
             catch (Exception e)
             {
                 Logger.Write(e.Message);
             }
         }

         //private static async Task MoveFile(string sourceFile, string destinationFile)
         //{
         //    try
         //    {
         //        var sourceDir = Path.GetDirectoryName(sourceFile);
         //        var destDir = Path.GetDirectoryName(destinationFile);
         //        if (string.CompareOrdinal(sourceDir, destDir) == 0)
         //        {
         //            await Task.Run(() => File.Move(sourceFile, destinationFile));
         //        }
         //        else
         //        {
         //            using (var sourceStream = File.Open(sourceFile, FileMode.Open))
         //            {
         //                using (var destinationStream = File.Create(destinationFile))
         //                {
         //                    await sourceStream.CopyToAsync(destinationStream);
         //                }
         //            }                    
         //        }
         //    }
         //    catch (Exception ex)
         //    {
         //        Logger.Write(ex.Message);
         //    }
         //}
     }
     */
    }

    public class SwapEntity
    {
        public SwapEntity(string path)
        {
            var defaultPath = Path.Combine(Settings.Default.LotroPath, Resources.DatFile);

            IsValid = File.Exists(path);
            IsActive = IsValid && string.CompareOrdinal(path, defaultPath) == 0;
            DatFilePath = path;
        }

        public bool IsActive { get; }

        public bool IsValid { get; }

        public string DatFilePath { get; }

        public bool IsInit => DatController != null && DatController.IsValid;

        public DatController DatController { get; private set; }

        public async Task InitAsync()
        {
            if (IsValid)
            {
                await InitControllerAsync();
            }
        }

        private async Task InitControllerAsync()
        {
            try
            {
                DatController = new DatController(DatFilePath);

                if (DatController.IsValid)
                {
                    await DatController.GetMarkAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
        }

        public void CloseController()
        {
            DatController?.Close();
            DatController = null;
        }
    }
}
