using System;
using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts
{
	/// <summary>
	/// Service for interaction with game database.
	/// Creates database and table in it if not exist.
	/// </summary>
	public class DataService
	{
		private readonly SQLiteConnection connection;

		public DataService(string databaseName)
		{
#if UNITY_EDITOR
			var dbPath = Path.Combine(Application.streamingAssetsPath, databaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = Path.Combine(Application.persistentDataPath, databaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb =
 new WWW("jar:file://" + Application.dataPath + "!/assets/" + databaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb =
 Application.dataPath + "/Raw/" + databaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#else
		var loadDb =
 Path.Combine(Application.streamingAssetsPath, databaseName);  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#endif
            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
			connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
			Debug.Log("Final PATH: " + dbPath);

			
			//TODO: clear table before deploying for the first time (with code or with hands?)
			try
			{
				connection.Table<Record>().ToList();
			}
			catch (Exception)
			{
				connection.CreateTable<Record>();
			}
		}

		/// <summary>
		/// Returns the iterator to the table with records.
		/// </summary>
		public IEnumerable<Record> GetRecords()
		{
			return connection.Table<Record>();
		}

		/// <summary>
		/// Updates the given record if it exists in table or inserts otherwise.
		/// </summary>
		public void UpdateRecord(Record record)
		{
			if (GetRecords().Any(r => r.Id == record.Id))
			{
				connection.Update(record);
			}
			else
			{
				connection.Insert(record);
			}
		}
	}
}