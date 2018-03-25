using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace Shop
{
    public partial class Form1 : Form
    {
        string ConnectionSrting;
        SqlConnection sqlConnetion = null;
        SqlDataAdapter sqlDataAdapter = null;
        DataSet dataSet = null;
        SqlCommand sqlCommand = null;
        public Form1()
        {
            InitializeComponent();
            try
            {
                ConnectionSrting = ConfigurationManager.ConnectionStrings[$"ConnectionString"].ConnectionString;
                sqlConnetion = new SqlConnection(ConnectionSrting);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            txtClient.TextChanged += TxtClient_TextChanged;
            txtModel.TextChanged += TxtClient_TextChanged;
            txtSeller.TextChanged += TxtClient_TextChanged;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                DGV_TableRange.DataSource = null;
                dataSet = new DataSet();
                sqlDataAdapter = new SqlDataAdapter("Select Id '№', Manufacturer Производитель, Model Модель, Price Цена, Quantity 'Кол-во' from dbo.Refrigerators", sqlConnetion);
                sqlDataAdapter.Fill(dataSet);
                DGV_TableRange.DataSource = dataSet.Tables[0];
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void TxtClient_TextChanged(object sender, EventArgs e)
        {
            //проверка на ввод всех данных
            if (!String.IsNullOrWhiteSpace(txtClient.Text) && !String.IsNullOrWhiteSpace(txtModel.Text) && !String.IsNullOrWhiteSpace(txtSeller.Text)) bttnGetOrder.Enabled = true;
        }
        private void bttnGetOrder_Click(object sender, EventArgs e)
        {
            //ТРАНЗАКЦИЯ
            sqlConnetion.Open();
            string manufacturer="", price="";
            SqlTransaction transaction = sqlConnetion.BeginTransaction();
            sqlCommand = sqlConnetion.CreateCommand();
            sqlCommand.Transaction = transaction;
            try
            {
                // выполняем две отдельные команды
                sqlCommand.CommandText = "insert into dbo.[ORDER] VALUES(@Buyer, @Seller, default, @ID);";
                sqlCommand.Parameters.Add("@Buyer", SqlDbType.VarChar, 50).Value = txtClient.Text;
                sqlCommand.Parameters.Add("@Seller", SqlDbType.VarChar, 50).Value = txtSeller.Text;
                sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = 1;
                sqlCommand.ExecuteNonQuery();
                //2 транзакция
                sqlCommand.CommandText = "UPDATE dbo.Refrigerators SET Quantity = Quantity - 1 WHERE Model = @model";
                sqlCommand.Parameters.Add("@model", SqlDbType.VarChar, 50).Value = txtModel.Text;
                sqlCommand.ExecuteNonQuery();
                // подтверждаем транзакцию
                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                transaction.Rollback();
            }
            finally { sqlConnetion.Close(); }

            //метод для возврата данных по покупке
            connect(out manufacturer, out price);
            //имитация печать чека
            MessageBox.Show($"Продавец: {txtSeller.Text}\nКлиент: {txtClient.Text}\nПроизводитель: {manufacturer}\nМодель: {txtModel.Text}\nЦена: {price}", "ЧЕК");
        }
        void connect(out string manufacturer, out string price) {
            DataTable table = dataSet.Tables[0];
            DataRow[] rows = table.Select();
            string price1 ="", manufacturer1="";
            // Print the value one column of each DataRow.
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i]["Модель"].ToString() == txtModel.Text)
                {
                    int key = i;
                     price1 = rows[key]["Цена"].ToString();
                     manufacturer1 = rows[key]["Производитель"].ToString();
                    break;
                }
            }
            price = price1;
            manufacturer = manufacturer1;
        }
    }
}
