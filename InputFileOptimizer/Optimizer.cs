using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

using System.IO;

namespace InputFileOptimizer
{
    public class Optimizer
    {
        public string input_file_path;
        public string output_directory;
        public int st_volume_limit;
        public int lt_volume_limit;
        public int xlt_volume_limit;
        public int xlt_subtract_volume;
        public bool sort_by_column = true;
        public int well_id_column;
        public int volume_column;


        public bool sss = true;
    


        public void optimizeFile()
        {

            string[] hit_list = File.ReadAllLines(input_file_path);

            //--Optimize the list if necessary
            if (sort_by_column )
            {
                hit_list = sortTheLIst(hit_list, well_id_column);
            }
            

            List<string> STHitList = new List<string>();
            STHitList.Add(hit_list[0]);
            List<string> LTHitList = new List<string>();
            LTHitList.Add(hit_list[0]);
            List<string> XLTHitList = new List<string>();
            XLTHitList.Add(hit_list[0]);

            for (int i = 1; i < hit_list.Length; ++i)
            {

                string[] line = hit_list[i].Split(',');

                float volume = float.Parse(line[volume_column]);

                
                //--Add Lines to an array for the ST Hit List
                if (volume <= st_volume_limit)
                {
                    STHitList.Add(ConvertStringArrayToString(line));

                }
                //--Add Lines to an array for the LT Hit List
                if (volume > st_volume_limit && volume <= lt_volume_limit)
                {

                    LTHitList.Add(ConvertStringArrayToString(line));

                }
                //--Add Lines to an array for the XLT Hit List
                if (volume > lt_volume_limit)
                {
                    //--Split the Transfer into two if the volume exceeds the xlt Threshold
                    if (volume > xlt_volume_limit)
                    {
                        float smallerVolume = volume - 100;
                        line[4] = smallerVolume.ToString();

                        XLTHitList.Add(ConvertStringArrayToString(line));

                        string[] new_line = line;

                        new_line[4] = xlt_subtract_volume.ToString();


                        XLTHitList.Add(ConvertStringArrayToString(line));
                    }
                    else
                    {
                        XLTHitList.Add(ConvertStringArrayToString(line));

                    }

                }
            }


            TextWriter twST = new StreamWriter(output_directory + "\\sthitlist.csv");

            foreach (String s in STHitList)
            {
                twST.WriteLine(s);
                
            }
            twST.Close();
            TextWriter twLT = new StreamWriter(output_directory + "\\lthitlist.csv");

            foreach (String s in LTHitList)
            {
                twLT.WriteLine(s);
               
            }
            twLT.Close();

            TextWriter twxLT = new StreamWriter(output_directory + "\\xlthitlist.csv");

            foreach (String s in XLTHitList)
            {
                twxLT.WriteLine(s);
                
            }
            twxLT.Close();
        }

        static string ConvertStringArrayToString(string[] array)
        {
            string result = string.Join(",", array);
            return result;
        }

        
        //--Reorder the Array Of lines so that transfers are optimized for a 96 well plate.
        static string[] sortTheLIst(string[] itemsToReorder, int wellIdIDX) {

            string[] arrayOfLetters ={"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"};


            List<string> arrayOfWellIDs = new List<string>();


             for (int i = 0; i < 24; ++i) {
 
                for (int j = 0; j < arrayOfLetters.Count(); ++j) {
 
                    string letter = arrayOfLetters[j];  
                    string wellID = letter + (i + 1);

                    arrayOfWellIDs.Add(wellID);
 
                }
          }
 
                 string[]  reorderedArrayOfIDS = new string[384];


                 for (var i = 1; i < itemsToReorder.Count(); ++i)
                 {
 
                 //--Gets new Position

                     string foundWellID = itemsToReorder[i].Split(',')[wellIdIDX];
                     int pos = arrayOfWellIDs.IndexOf(foundWellID);
                     reorderedArrayOfIDS[pos] = itemsToReorder[i];
 
                 }

                 //-- Add Header back to file
                 //reorderedArrayOfIDS[0] = itemsToReorder[0];
                 //--Remove Empty indexs
                
              
                 reorderedArrayOfIDS = reorderedArrayOfIDS.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                
                 for (var i = 1; i < itemsToReorder.Count(); ++i)
                 {
                     itemsToReorder[i] = reorderedArrayOfIDS[i-1];

                 }


                 return itemsToReorder;

        
        
        
        }


    }







    }
