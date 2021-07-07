using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using DBXML.Dados;
using ClassLibrary;

namespace DBXML
{
    public partial class FormMarket : Form
    {
       
        private WinFormModel winf;

     

        public FormMarket()
        {			
            InitializeComponent();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            winf = new WinFormModel();
            LimparTextBox(this);
            btnSalvar.Enabled = true;
            GetDados();



        }

        private void LimparTextBox(Control con)
        {
            foreach(Control c in con.Controls)
            {
                if (c is TextBox)
                    ((TextBox)c).Clear();
                else
                    LimparTextBox(c);
            }
        }

      
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                using (DataSet dsResultado = new DataSet())
                {
                    dsResultado.ReadXml(WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml");
                    if (dsResultado.Tables.Count == 0)
                    {
                        //cria uma instância do Produto e atribui valores às propriedades
                        Produto oProduto = new Produto();
                        oProduto.Codigo = Convert.ToInt32(txtCodigoProduto.Text);
                        oProduto.Nome = txtNomeProduto.Text;
                        oProduto.Preco = Convert.ToDecimal(txtPreco.Text);
                        oProduto.Estoque = Convert.ToInt32(txtEstoque.Text);
                        oProduto.Descricao = txtDescricao.Text;
                        XmlTextWriter writer = new XmlTextWriter(WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml", System.Text.Encoding.UTF8);
                        writer.WriteStartDocument(true);
                        writer.Formatting = Formatting.Indented;
                        writer.Indentation = 2;
                        writer.WriteStartElement("Produtos");
                        writer.WriteStartElement("Produto");
                        writer.WriteStartElement("Codigo");
                        writer.WriteString(oProduto.Codigo.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("Nome");
                        writer.WriteString(oProduto.Nome);
                        writer.WriteEndElement();
                        writer.WriteStartElement("Preco");
                        writer.WriteString(oProduto.Preco.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("Estoque");
                        writer.WriteString(oProduto.Estoque.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("Descricao");
                        writer.WriteString(oProduto.Descricao);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Close();
                        dsResultado.ReadXml(WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml");
                    }
                    else
                    {
                        //inclui os dados no DataSet
                        dsResultado.Tables[0].Rows.Add(dsResultado.Tables[0].NewRow());
                        dsResultado.Tables[0].Rows[dsResultado.Tables[0].Rows.Count - 1]["Codigo"] = txtCodigoProduto.Text;
                        dsResultado.Tables[0].Rows[dsResultado.Tables[0].Rows.Count - 1]["Nome"] = txtNomeProduto.Text.ToUpper();
                        dsResultado.Tables[0].Rows[dsResultado.Tables[0].Rows.Count - 1]["Preco"] = txtPreco.Text;
                        dsResultado.Tables[0].Rows[dsResultado.Tables[0].Rows.Count - 1]["Estoque"] = txtEstoque.Text;
                        dsResultado.Tables[0].Rows[dsResultado.Tables[0].Rows.Count - 1]["Descricao"] = txtDescricao.Text;
                        dsResultado.AcceptChanges();
                        //--  Escreve para o arquivo XML final usando o método Write
                        dsResultado.WriteXml(WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml", XmlWriteMode.IgnoreSchema);
                    }
                    //exibe os dados no datagridivew 
                    dgvDados.DataSource = dsResultado.Tables[0];
                    MessageBox.Show("Dados salvos com sucesso.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnCarregar_Click(object sender, EventArgs e)
        {
            GetDados();
        }


        public void GetDados()
        {
            try
            {
                DataSet dsResultado = new DataSet();
                dsResultado.ReadXml(WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml");
                if (dsResultado.Tables.Count != 0)
                {
                    if (dsResultado.Tables[0].Rows.Count > 0)
                    {
                        dgvDados.DataSource = dsResultado.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Load((WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml"));
            var prods = from p in doc.Descendants("Produto")
                        select new
                        {
                            NomeProduto = p.Element("Nome").Value,
                            PrecoProduto = p.Element("Preco").Value,
                        };
            foreach (var p in prods)
            {
                lbProdutos.Items.Add(p.NomeProduto + " " + p.PrecoProduto);
            }
        }

        private void btnLocalizar_Click(object sender, EventArgs e)
        {
            var prods = from p in XElement.Load((WinFormModel.CaminhoDadosXML(winf.Caminho) + @"Dados\Produtos.xml")).Elements("Produto")
                        where p.Element("Codigo").Value == txtCodigoProduto.Text
                        select new
                        {
                            NomeProduto = p.Element("Nome").Value,
                            PrecoProduto = p.Element("Preco").Value,
                            EstoqueProduto = p.Element("Estoque").Value,
                            DescricaoProduto = p.Element("Descricao").Value,
                        };
            // Executa a consulta
            foreach (var produto in prods)
            {
                txtNomeProduto.Text = produto.NomeProduto;
                txtPreco.Text = produto.PrecoProduto;
                txtEstoque.Text = produto.EstoqueProduto;
                txtDescricao.Text = produto.DescricaoProduto;
            }
        }

        private void dgvDados_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //obtem o valor das células do grid
                txtCodigoProduto.Text = dgvDados.CurrentRow.Cells[0].Value.ToString();
                txtNomeProduto.Text = dgvDados.CurrentRow.Cells[1].Value.ToString();
                txtPreco.Text = dgvDados.CurrentRow.Cells[2].Value.ToString();
                txtEstoque.Text = dgvDados.CurrentRow.Cells[3].Value.ToString();
                txtDescricao.Text = dgvDados.CurrentRow.Cells[4].Value.ToString();
            }
            catch
            { }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
