using MySql.Data.MySqlClient;
using System.Data;
using UnityEngine;

public class ConnectMySql : MonoBehaviour
{
    void Start()
    {

        //���ݿ��ַ���˿ڡ��û��������ݿ���������
        string sqlSer = "server = 127.0.0.1;port = 3306;user= root;database = mygametest;password = ;charset=utf8mb4";
        //��������
        MySqlConnection conn = new MySqlConnection(sqlSer);
        try
        {
            conn.Open();
            Debug.Log("------���ӳɹ�------");
            //sql���
            string sqlQuary = "SELECT * FROM gametestuserdata;";

            Debug.Log(sqlQuary);

            MySqlCommand comd = new MySqlCommand(sqlQuary, conn);

            MySqlDataReader reader = comd.ExecuteReader();

            while (reader.Read())
            {
                //ͨ��reader������ݿ���Ϣ
                Debug.Log(reader.GetString("�û���"));
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error:" + e.Message);
        }
        finally
        {
            conn.Close();
        }
    }
}
