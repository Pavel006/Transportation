using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace RAx
{
    public partial class Form1 : Form
    {
        public string connectString1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Av_Bl1.mdb;";
        Connection c = new Connection();
        private int act_table = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            button1_Click(sender, e);
            Get_Bilets();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string CommandText = "SELECT " + "[Перевозка].[Номер], " + "[Маршрут].[Номер маршрута], " + "[Маршрут].[Пункт назначения], " + "[Маршрут].[Время отправки], " + "[Маршрут].[Время прибытия], " + "[Билет].[Место], " + "[Билет].[Ф_И_О], " + "[Билет].[Стоимость], " + "[Водитель].[Ф_И_О] " + "FROM " + "[Перевозка], " + "[Маршрут], " + "[Билет], " + "[Водитель] " + "WHERE " + "([Перевозка].[ID_Marshrut]=[Маршрут].[ID_Marshrut]) AND " + "([Перевозка].[ID_Bilet] = [Билет].[ID_Bilet]) AND " + "([Перевозка].[ID_Voditel] = [Водитель].[ID_Voditel]) ";
            if (textBox1.Text != "") // если набран текст в поле фильтра
            {
                if (comboBox1.SelectedIndex == 0) // № перевозки
                    CommandText = CommandText + " AND ([Перевозка].[Номер] = '" + textBox1.Text + "')";
                if (comboBox1.SelectedIndex == 1) // № маршрута
                    CommandText = CommandText + " AND (Маршрут.[Номер маршрута] = '" + textBox1.Text + "') ";
                if (comboBox1.SelectedIndex == 2) // Пункт назначения
                    CommandText = CommandText + " AND (Маршрут.[Пункт назначения] LIKE '" + textBox1.Text + "%') ";
                if (comboBox1.SelectedIndex == 3) // Пассажир
                    CommandText = CommandText + " AND (Билет.[Ф_И_О] LIKE '" + textBox1.Text + "%') ";
                if (comboBox1.SelectedIndex == 4) // Водитель
                    CommandText = CommandText + " AND ([Водитель].[Ф_И_О] LIKE '" + textBox1.Text + "%') ";
            }
            c.SIDU(CommandText);

            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(CommandText, connectString1);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds,  "[Перевозка]");
            dataGridView1.DataSource = ds.Tables["[Перевозка]"].DefaultView;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string CommandText;
            string num_per, ID_M, ID_B, ID_D, ID_A, ID_V;
            int row;
            Form2 f = new Form2(); // создали новую форму
            if (f.ShowDialog() == DialogResult.OK)
            {
                // добавляем данные // Номер перевозки
                if (f.textBox1.Text == "") num_per = "0";
                else num_per = f.textBox1.Text; // добавляем ID_Marshrut
                row = f.dataGridView1.CurrentCell.RowIndex; // взяли строку с dataGridView1
                ID_M = Convert.ToString(f.dataGridView1[0, row].Value); // добавляем 
                row = f.dataGridView2.CurrentCell.RowIndex; // взяли строку с dataGridView2 
                ID_B = Convert.ToString(f.dataGridView2[0, row].Value); // добавляем ID_Dispetcher
                row = f.dataGridView3.CurrentCell.RowIndex; // взяли строку с dataGridView3 
                ID_D = Convert.ToString(f.dataGridView3[0, row].Value); // добавляем ID_Avtobus
                row = f.dataGridView4.CurrentCell.RowIndex; // взяли строку с dataGridView4 
                ID_A = Convert.ToString(f.dataGridView4[0, row].Value); // добавляем ID_Voditel 
                row = f.dataGridView5.CurrentCell.RowIndex; // взяли строку с dataGridView5 
                ID_V = Convert.ToString(f.dataGridView5[0, row].Value); // формируем CommandText
                CommandText = "INSERT INTO [Перевозка] (Номер, ID_Marshrut, ID_Bilet, ID_Dispetcher, ID_Avtobus, ID_Voditel) " + "VALUES (" + num_per + ", " + ID_M + ", " + ID_B + ", " + ID_D + ", " + ID_A + ", " + ID_V + ")"; // выполняем SQL-команду
                My_Execute_Non_Query(CommandText); // перерисовываем dataGridView1 
                button1_Click(sender, e);
            }
        }
        public void My_Execute_Non_Query(string CommandText) { OleDbConnection conn = new OleDbConnection(connectString1); conn.Open(); OleDbCommand myCommand = conn.CreateCommand(); myCommand.CommandText = CommandText; myCommand.ExecuteNonQuery(); conn.Close(); }
        private void button3_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            if (f.ShowDialog() == DialogResult.OK)
            {
                int index, index_old;
                string ID;
                string CommandText = "DELETE FROM ";
                index = dataGridView1.CurrentRow.Index; // № по порядку в таблице представления 
                index_old = index;
                ID = Convert.ToString(dataGridView1[0, index].Value); // ID подаем в запрос как строку 
                                                                      // Формируем строку CommandText 
                CommandText = "DELETE FROM [Перевозка] WHERE [Перевозка].[Номер] = '" + ID + "'"; // выполняем SQL-запрос 
                My_Execute_Non_Query(CommandText); // перерисовывание dbGridView1 
                button1_Click(sender, e);
                if (index_old >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1[0, index_old].Selected = true;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Get_Bilets(); act_table = 1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Get_Marshruts(); act_table = 2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Get_Avtobus(); act_table = 3;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Get_Voditel(); act_table = 4;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Get_Dispetcher(); act_table = 5;
        }
        private void Get_Bilets() // читает все поля из таблицы "Билет" 
        {
            string CommandText = "SELECT ID_Bilet, [Место], [Стоимость], [Время], [Ф_И_О], [Паспорт], [Льготы] FROM [Билет]";
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(CommandText, connectString1); // создаем объект DataSet 
            DataSet ds = new DataSet(); // заполняем dataGridView1 данными из таблицы "Билет" базы данных 
            dataAdapter.Fill(ds, "[Билет]");
            dataGridView2.DataSource = ds.Tables[0].DefaultView;
            dataGridView2.Columns[0].Visible = false; // Прячем поле ID_Bilets 
        }
            private void Get_Marshruts() // читает все поля из таблицы "Маршрут" { string CommandText = "SELECT * FROM [Маршрут]";
        {
            string CommandText = "SELECT * FROM [Маршрут]";
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(CommandText, connectString1);
                DataSet ds = new DataSet(); // создаем объект DataSet 
                dataAdapter.Fill(ds, "[Маршрут]");
                dataGridView2.DataSource = ds.Tables[0].DefaultView;
            }
        
            private void Get_Avtobus() // читает все поля из таблицы "Автобус" 
            {
            string CommandText = "SELECT * FROM Автобус";
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(CommandText, connectString1);
            DataSet ds = new DataSet(); // создаем объект DataSet
            dataAdapter.Fill(ds, "Автобус"); // заполняем набор данных данными из таблицы "Автобус"
            dataGridView2.DataSource = ds.Tables[0].DefaultView; dataGridView2.Columns[0].Visible = false; // спрятать нулевой столбец (поле ID_Avtobus) 
            }
        private void Get_Voditel() // читает все поля из таблицы "Водитель" 
        {
         string CommandText = "SELECT * FROM Водитель";
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(CommandText, connectString1);
            DataSet ds = new DataSet(); dataAdapter.Fill(ds, "Водитель");
            dataGridView2.DataSource = ds.Tables[0].DefaultView;
            dataGridView2.Columns[0].Visible = false; 
        }
        private void Get_Dispetcher()
        {
         string CommandText = "SELECT * FROM [Диспетчер]";
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(CommandText, connectString1);
            DataSet ds = new DataSet(); dataAdapter.Fill(ds, "Диспетчер");
            dataGridView2.DataSource = ds.Tables[0].DefaultView;
            dataGridView2.Columns[0].Visible = false;
        }
        private void Add_Bilet(string mesto, string stoimost, DateTime vremja, string name, string passport, bool lgota)
        {
            string CommandText;
            string s_vremja;
            string s_stoimost;
            s_vremja = Convert.ToString(vremja); // переводим время в строку 
            s_stoimost = stoimost.Replace(',', '.'); // меняем разделитель разрядов '.' на // разделитель понятный синтаксису SQL '.'
            CommandText = "INSERT INTO [Билет] (Место, Стоимость, [Время], [Ф_И_О], Паспорт, Льготы) " + "VALUES ('" + mesto + "', " + s_stoimost + ", '" + s_vremja + "', '" + name + "', '" + passport + "', " + lgota + ")";
            My_Execute_Non_Query(CommandText);
        }
        private void Add_Marshrut(string num_marsh, string punkt, string rajon, string oblast, double rasst, double ves, DateTime vremja_otpr, DateTime vremja_prib)
        {
            string CommandText;
            string s_otpr, s_prib;
            string s_ves, s_rasst;
            s_otpr = Convert.ToString(vremja_otpr); // переводим время отправки в строку 
            s_prib = Convert.ToString(vremja_prib); // переводим время прибытия в строку 
            s_ves = Convert.ToString(ves); // переводим вес из double в строку 
            s_ves = s_ves.Replace(',','.'); // меняем запятую на точку согласно синтаксису SQL 
            s_rasst = Convert.ToString(rasst); // переводим расстояние из double в string 
            s_rasst = s_rasst.Replace(',', '.'); // меняем запятую на точку 
            CommandText = "INSERT INTO [Маршрут] ([Номер маршрута], [Пункт назначения], Район, Область, Расстояние, Вес, [Время отправки], [Время прибытия])" + " VALUES ('" + num_marsh + "', '" + punkt + "', '" + rajon + "', '" + oblast + "', " + s_rasst + ", " + s_ves + ", '" + s_otpr + "', '" + s_prib + "')";
            My_Execute_Non_Query(CommandText);
        }
        void Add_Avtobus(string num, string model, string znak, string k_mest)
        {
            string CommandText;
            CommandText = "INSERT INTO [Автобус] ([Номер], [Модель], [Номерной знак], [Количество мест])" + " VALUES ('" + num + "', '" + model + "', '" + znak + "', " + k_mest + ")";
            My_Execute_Non_Query(CommandText);
        }
        void Add_Voditel(string f_i_o, string d_r, string passport) // добавить водителя 
        {
            string CommandText;
            CommandText = "INSERT INTO [Водитель] ([Ф_И_О], [Дата рождения], [Паспорт])" + " VALUES ('" + f_i_o + "', '" + d_r + "', '" + passport + "')";
            My_Execute_Non_Query(CommandText); 
        }
        void Add_Dispetcher(string f_i_o, string d_r, string adres) // добавить диспетчера 
        {
            string CommandText;
            CommandText = "INSERT INTO [Диспетчер] ([Ф_И_О], [Дата рождения], [Адрес])" + " VALUES ('" + f_i_o + "', '" + d_r + "', '" + adres + "')";
            My_Execute_Non_Query(CommandText);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (act_table == 1) // обрабатываем таблицу "Билет" 
            {
                Form4 f = new Form4();
                if (f.ShowDialog() == DialogResult.OK)
                { // добавляем данные в таблицу "Билеты" 
                    Add_Bilet(f.textBox1.Text, f.textBox2.Text, Convert.ToDateTime(f.textBox3.Text), f.textBox4.Text, f.textBox5.Text, f.checkBox1.Checked);
                    Get_Bilets();
                }
            }
            else if (act_table == 2) // обрабатываем таблицу "Маршрут" 
            { Form5 f = new Form5(); if (f.ShowDialog() == DialogResult.OK)
                { // добавляем данные в таблицу "Маршрут" 
                    Add_Marshrut(f.textBox1.Text, f.textBox2.Text, f.textBox3.Text, f.textBox4.Text, Convert.ToDouble(f.textBox5.Text), Convert.ToDouble(f.textBox6.Text), f.dateTimePicker1.Value, f.dateTimePicker2.Value);
                    Get_Marshruts();
                }
             }
            else if (act_table == 3) // обрабатываем таблицу "Автобус"
            {
                Form6 f = new Form6();
                if (f.ShowDialog() == DialogResult.OK)
                { // добавляем данные в таблицу "Автобус"
                    Add_Avtobus(f.textBox1.Text, f.textBox2.Text, f.textBox3.Text, f.textBox4.Text);
                    Get_Avtobus();
                }
            }
            else if (act_table == 4) // обрабатываем таблицу "Водитель" 
            {
                Form7 f = new Form7();
                if (f.ShowDialog() == DialogResult.OK)
                { // добавляем данные в таблицу "Водитель" 
                    Add_Voditel(f.textBox1.Text, Convert.ToString(f.dateTimePicker1.Value), f.textBox2.Text)
                        ; Get_Voditel();
                }
            }
            else if (act_table == 5) // обрабатываем таблицу "Диспетчер" 
            {
                Form8 f = new Form8();
                if (f.ShowDialog() == DialogResult.OK)
                { // добавляем данные в таблицу "Диспетчер" 
                    Add_Dispetcher(f.textBox1.Text, Convert.ToString(f.dateTimePicker1.Value), f.textBox2.Text);
                    Get_Dispetcher();
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3(); f.Text = " ";
            if (f.ShowDialog() == DialogResult.OK)
            {
                int index, index_old;
                string ID;
                string CommandText = "DELETE FROM ";
                index = dataGridView2.CurrentRow.Index; // № по порядку в таблице представления 
                index_old = index; ID = Convert.ToString(dataGridView2[0, index].Value); // ID подаем в запрос как строку
                                                                                         // Формируем строку CommandText 
                if (act_table == 1) // обрабатываем таблицу "Билет" 
                    CommandText = "DELETE FROM Билет WHERE Билет.ID_Bilet = " + ID;
                if (act_table == 2) // обрабатываем таблицу "Маршрут" 
                    CommandText = "DELETE FROM Маршрут WHERE Маршрут.ID_Marshrut = " + ID;
                if (act_table == 3) // обрабатываем таблицу "Автобус" 
                    CommandText = "DELETE FROM Автобус WHERE Автобус.ID_Avtobus = " + ID;
                if (act_table == 4) // обрабатываем таблицу "Водитель" 
                    CommandText = "DELETE FROM Водитель WHERE Водитель.ID_Voditel = " + ID;
                if (act_table == 5) // обрабатываем таблицу "Диспетчер"
                    CommandText = "DELETE FROM Диспетчер WHERE Диспетчер.ID_Dispetcher = " + ID;
                // выполняем SQL-запрос 
                My_Execute_Non_Query(CommandText); // перерисовывание dbGridView2 
                if (act_table == 1) Get_Bilets();
                else if (act_table == 2) Get_Marshruts();
                else if (act_table == 3) Get_Avtobus();
                else if (act_table == 4) Get_Voditel();
                else if (act_table == 5) Get_Dispetcher();

                if (index_old >= 0)
                {
                    dataGridView2.ClearSelection();
                    dataGridView2[0, index_old].Selected = true;
                }
            }
            }
    }
}
