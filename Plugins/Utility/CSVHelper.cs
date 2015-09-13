using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class CSVHelper : MonoBehaviour {

	public static string GetWebpage(string url)
	{
		//Switch to standalone as we need to be able to do unrestricted WWW calls: Google does not host a crossdomain.xml
		/*
		if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WebPlayer)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
		}
		*/
		WWW wwwReq = new WWW(url);
		while (!wwwReq.isDone) { }
		if (wwwReq.error == null)
		{
			return wwwReq.text;
		}
		else
		{
			Debug.LogWarning("Error grabbing gDocs data:" + wwwReq.error);
			Debug.LogWarning("Trying again via proxy. Switch to standalone target to prevent this!");
			/*
			//Unity Editor needs Crossdomain.xml when running in Editor with Webplayer target
			//If you are concerned about privacy/performance: You can also host this proxy yourself, see PDF
			Debug.LogWarning("Error grabbing gDocs data:" + wwwReq.error);
			Debug.LogWarning("Trying again via proxy. Switch to standalone target to prevent this!");
			
			WWWForm form = new WWWForm();
			form.AddField("page", url);
			WWW wwwReq2 = new WWW(urlRequestProxy, form);
			while (!wwwReq2.isDone) { }
			if (wwwReq2.error == null)
			{
				return wwwReq2.text;
			}
			else
			{
				Debug.LogError(wwwReq2.error);
			}
			*/
		}
		return "";
		
	}
	
	public static List<string> GetCVSLines(string data)
	{
		List<string> lines = new List<string>();
		int i = 0;
		int searchCloseTags = 0;
		int lastSentenceStart = 0;
		while (i < data.Length)
		{
			if (data[i] == '"')
			{
				if (searchCloseTags == 0)
					searchCloseTags++;
				else
					searchCloseTags--;
			}
			else if (data[i] == '\n')
			{
				if (searchCloseTags == 0)
				{
					lines.Add(data.Substring(lastSentenceStart, i - lastSentenceStart));
					lastSentenceStart = i + 1;
				}
			}
			i++;
		}
		if (i - 1 > lastSentenceStart)
		{
			lines.Add(data.Substring(lastSentenceStart, i - lastSentenceStart));
		}
		return lines;
	}
	
	static string PregReplace(string input, string[] pattern, string[] replacements)
	{
		if (replacements.Length != pattern.Length)
			throw new ArgumentException("Replacement and Pattern Arrays must be balanced");
		
		for (var i = 0; i < pattern.Length; i++)
		{
			input = Regex.Replace(input, pattern[i], replacements[i]);
		}
		
		return input;
	}
	
	public static string CleanData(string data)
	{
		//Debug.Log("EBFORE="+data);
		//Cut of formula data
		int formulaIndex = data.IndexOf("\n\n\n[");
		if (formulaIndex != -1) data = data.Substring(0, formulaIndex);
		
		string[] patterns = new string[4];
		string[] replacements = new string[4];
		int patrs = 0;
		int reps = 0;
		
		patterns[patrs++] = @" \[[0-9]+\],";
		replacements[reps++] = ",";
		
		patterns[patrs++] = @" \[[0-9]+\]""";
		replacements[reps++] = "\"";
		
		patterns[patrs++] = @" \[[0-9]+\]([\n\r$]+)";
		replacements[reps++] = "$1";
		patterns[patrs++] = @" \[[0-9]+\]\Z";
		replacements[reps++] = "";
		
		data = PregReplace(data, patterns, replacements);
		
		
		
		return data;
		
	}
	
	
	public static List<string> GetCVSLine(string line)
	{
		List<string> list = new List<string>();
		int i = 0;
		int searchCloseTags = 0;
		int lastEntryBegin = 0;
		while (i < line.Length)
		{
			if (line[i] == '"')
			{
				if (searchCloseTags == 0)
					searchCloseTags++;
				else
					searchCloseTags--;
			}
			else if (line[i] == ',')
			{
				if (searchCloseTags == 0)
				{
					list.Add(StripQuotes(line.Substring(lastEntryBegin, i - lastEntryBegin)));
					lastEntryBegin = i + 1;
				}
			}
			i++;
		}
		if (line.Length > lastEntryBegin)
		{
			list.Add(StripQuotes(line.Substring(lastEntryBegin)));//Add last entry
		}
		return list;
	}
	
	//Remove the double " that CVS adds inside the lines, and the two outer " as well
	public static string StripQuotes(string input)
	{
		if (input.Length < 1 || input[0] != '"') return input;//Not a " formatted line
		
		string output = ""; ;
		int i = 1;
		bool allowNextQuote = false;
		while (i < input.Length - 1)
		{
			string curChar = input[i] + "";
			if (curChar == "\"")
			{
				if (allowNextQuote)
					output += curChar;
				allowNextQuote = !allowNextQuote;
			}
			else
			{
				output += curChar;
			}
			i++;
		}
		return output;
	}
}
