using UnityEngine;
using System.Collections;
using UnityEditor;
public class MyTools{
	[MenuItem("MyTools/Clear PlayerPrefs")]
	private static void NewMenuOption()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("MyTools/Screen Shot &a")]
	private static void GetScrShot()
	{
		// File path
		string folderPath = "D:/screenshots/";
		string fileName = "scr";
		
		// Create the folder beforehand if not exists
		if(!System.IO.Directory.Exists(folderPath))
			System.IO.Directory.CreateDirectory(folderPath);
		int i = 0;
		while (System.IO.File.Exists(folderPath + fileName + ".png")) {
			fileName = "scr" + i;
			i++;
		}

		// Capture and store the screenshot
		ScreenCapture.CaptureScreenshot(folderPath + fileName + ".png");
	}
}
