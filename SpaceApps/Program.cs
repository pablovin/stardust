using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceApps
{
    class Program
    {
        static void Main(string[] args)
        {

            DatasetServicoLibrary.DataSetSvc svc = new DatasetServicoLibrary.DataSetSvc();

            svc.GetDataSetImage("", "");

        }
        

    }
}
