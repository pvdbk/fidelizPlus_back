﻿using System;

namespace fidelizPlus_back.AppDomain
{
	public partial class Comment : RelatedToBoth
	{
		public string Text { get; set; }
		public int Rating { get; set; }
	}
}
