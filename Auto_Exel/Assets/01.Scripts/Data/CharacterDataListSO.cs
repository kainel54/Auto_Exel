using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Character/List")]
public class CharacterDataListSO : ScriptableObject, IToJson, IEnumerable<CharacterDataSO>
{
    public List<CharacterDataSO> list;

    public CharacterDataSO this[int i]
    {
        get => list[i];
    }

    public string ToJson()
    {
        return ToJsonByData(list);
    }

    public static string ToJsonByData(List<CharacterDataSO> list)
    {
        JSONObject obj = new JSONObject();
        JSONObject arr = new JSONObject();

        foreach (var data in list)
        {
            JSONObject charData = new JSONObject(data.ToJson());
            arr.Add(charData);
        }

        obj.AddField("list", arr);
        return obj.ToString();
    }

    public CharacterDataSO FindCharDataByGUID(string guid)
    {
        return list.Find(x => x.guid == guid);
    }

    public IEnumerator<CharacterDataSO> GetEnumerator()
    {
        for (int i = 0; i < list.Count; i++)
            yield return list[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }

    public int Count => list.Count;
}