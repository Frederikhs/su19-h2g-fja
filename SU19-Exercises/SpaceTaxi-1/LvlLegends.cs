using System;
using System.Collections.Generic;

namespace SpaceTaxi_1 {
    public class LvlLegends {
        public Dictionary<char, string> LegendsDic { get; }

        // LvlLegends is a dictionary of ASCII chars and their corresponding .png file name 
        // The constructors input determines of which lvl a dictionary is made
        public LvlLegends(string levelString) {
            LegendsDic = new Dictionary<char, string>();
            var legendsString = new TextLoader(levelString).get_lvl_legends();
            foreach (var elem in legendsString) {
                LegendsDic.Add(elem[0], elem.Substring(2));
            }
            
        }
        
    }
}