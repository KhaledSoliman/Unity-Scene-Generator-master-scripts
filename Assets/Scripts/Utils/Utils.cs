using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

static public class Utils {
    public enum PassengerIndex {
        GENDER = 0,
        AGE = 1
    }

    public enum CarIndex {
        Type = 0,
        Color = 1
    }

    [Serializable]
    public class RangeInt {
        public int start;
        public int length;
        public int end => start + length;

        public RangeInt(int start, int length) {
            this.start = start;
            this.length = length;
        }
    }

    [Serializable]
    public class Distribution {
        public RangeInt range;
    }

    public static void FillPath(string[] path, PassengerIndex index, string path_value, string exception_msg) {
        if ((int) index > Enum.GetNames(index.GetType()).Length)
            throw new Exception(exception_msg);

        path[(int) index] = path_value;
    }

    public static void FillPath(string[] path, CarIndex ind, string path_value, string exception_msg) {
        if ((int) ind > Enum.GetNames(ind.GetType()).Length)
            throw new Exception(exception_msg);

        path[(int) ind] = path_value;
    }

    public static string SelectModel(string basePath, string filterPath, List<string> disallowed = null) {
        var directoryPath = Application.dataPath + "/Resources/" + basePath + filterPath;

        var files = Directory.GetFiles(directoryPath).Where(path => {
            var permitted = !path.Contains(".meta");
            if (disallowed == null) return permitted;

            foreach (var disallowedFile in disallowed.Where(disallowedFile => disallowedFile == Path.GetFileName(path))
            ) {
                permitted = false;
            }

            return permitted;
        }).ToArray();
        
        Debug.Log(files);
        return Path.GetFileName(files[Random.Range(0, files.Length)]);
    }

    public static int SelectProperty(RangeInt range) {
        return Random.Range(range.start, range.length);
    }

    public static Vector3 SelectVector3(Vector3[] range) {
        return new Vector3(Random.Range(range[0].x, range[1].x), Random.Range(range[0].y, range[1].y),
            Random.Range(range[0].z, range[1].z));
    }

    /*static public void FillPath(string[] path, Enum ind, string path_value, string exception_msg)
    {
        int i = (int)ind;
        int i = (int)Enum.GetValues(ind.GetType()).GetValue(ind);
        if (ind.GetType().GetValues()[(int)ind] > Enum.GetNames(ind.GetType()).Length)
            throw new Exception(exception_msg);

        path[(int)ind] = path_value;
    }*/
}