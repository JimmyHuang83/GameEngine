using UnityEngine;
using System.Collections;
namespace GameEngine
{
	public class GameMath
	{
		public static int[][] initMaxtrix2(int r, int c, int initV)
		{
			int[][] mm = new int[r][];
			for(int i = 0; i < r; i++)
			{
				mm[i] = new int[c];
				for(int j = 0; j < c; j++)
				{
					mm[i][j] = initV;
				}
			}
			return mm;
		}

		public static int[][] RotateMaxtrix(int[][] matx, int angle = 0) // 0:0,1:90,2,180,3:270
		{

			int r, c;
			int[][] newMat;
			if (angle == 1)
			{
				r = matx[0].Length;
				c = matx.Length;
				newMat = new int[r][];
				for (int i = 0; i < r; i++)
				{
					newMat[i] = new int[c];
					for (int j = 0; j < c; j++)
					{
						newMat[i][j] = matx[c-j-1][i];
					}
				}
			}
			else if (angle == 2)
			{
				r = matx.Length;
				c = matx[0].Length;
				newMat = new int[r][];
				for (int i = 0; i < r; i++)
				{
					newMat[i] = new int[c];
					for (int j = 0; j < c; j++)
					{
						newMat[i][j] = matx[r-i-1][c-j-1];
					}
				}
			}
			else if (angle == 3)
			{
				r = matx[0].Length;
				c = matx.Length;
				newMat = new int[r][];
				for (int i = 0; i < r; i++)
				{
					newMat[i] = new int[c];
					for (int j = 0; j < c; j++)
					{
						newMat[i][j] = matx[c-j-1][r-i-1];
					}
				}
			}
			else
			{
				r = matx.Length;
				c = matx[0].Length;
				newMat = new int[r][];
				for (int i = 0; i < r; i++)
				{
					newMat[i] = new int[c];
					for (int j = 0; j < c; j++)
					{
						newMat[i][j] = matx[i][j];
					}
				}
			}

			return newMat;
		}

		public static T ConvertNullType<T>()
		{
			T inst = default(T); 
			return inst;
		}

		public static bool IsEqual(int[][] a, int[][] b)
		{
			if (a.Length != b.Length)
				return false;
			if (a [0].Length != b [0].Length)
				return false;
			for (int i = 0; i < a.Length; i++)
			{
				for (int j = 0; j < a[i].Length; j++)
				{
					if (a[i][j] != b[i][j])
						return false;
				}
			}
			return true;
		}
	}

}
