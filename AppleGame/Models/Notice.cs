///////////////////////////////////////////////////////////
//  Notice.cs
//  Implementation of the Class Notice
//  Generated by Enterprise Architect
//  Created on:      16-11��-2016 19:47:11
//  Original author: ant_d
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;



using AntDesigner.GameCityBase.interFace;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntDesigner.GameCityBase
{
	public class Notice  {

		public string Content{ get;  set;} 

		public Notice(){

		}
        public Notice(string content_) {
            Content = content_;
        }

		public int Id{
			get;
			set;
		}
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime
        {
            get; set;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifyTime
        { get; set; }

    }//end Notice

}//end namespace AntDesigner.AppleGame