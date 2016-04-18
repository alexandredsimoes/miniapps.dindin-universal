using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DinDinPro.Universal.Models
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection _conexao;


        public DataService()
        {
            _conexao = new SQLiteAsyncConnection(Constantes.DatabasePath, SQLiteOpenFlags.ReadWrite);
        }
        public SQLiteAsyncConnection Contexto
        {
            get
            {
                return _conexao;
            }
        }

        public async Task InicializarBaseDeDados(string[] tagsReceita, string[] tagsDespesa, string[] formasPagamento, int versao)
        {
            bool existe = false;
            try
            {
                var dbFile = await StorageFile.GetFileFromPathAsync(Constantes.DatabasePath);
                existe = true;
            }
            catch (FileNotFoundException)
            {
                existe = false;
            }

            if (!existe)
            {

                ////Pega o script de inicialização
                //var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Scripts/Script.txt"));
                
                //using (var sr = await file.OpenStreamForReadAsync())
                //{
                //    var buffer = new byte[sr.Length];
                //    var contentBytes = await sr.ReadAsync(buffer, 0, (int) sr.Length);
                //    var content = System.Text.Encoding.UTF8.GetString(buffer, 0, (int)sr.Length);
                //    await sr.FlushAsync();
                                                                             
                //    await ApplicationData.Current.LocalFolder.CreateFolderAsync("database", CreationCollisionOption.ReplaceExisting);
                //    SQLiteConnection script = new SQLiteConnection(Constantes.DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
                //    script.
                //    var command = script.CreateCommand(content);
                //    command.ExecuteNonQuery();
                    
                    
                //}
                //return;
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("database", CreationCollisionOption.ReplaceExisting);
                SQLiteAsyncConnection db = new SQLiteAsyncConnection(Constantes.DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, false);

                await db.CreateTableAsync<Configuracao>();
                await db.CreateTableAsync<Conta>();
                await db.CreateTableAsync<Lancamento>();
                await db.CreateTableAsync<LancamentoTag>();
                await db.CreateTableAsync<Tag>();
                await db.CreateTableAsync<FormaPagamento>();

                Action<string, string> criarTags = async (nome, tipo) =>
                {
                    await db.InsertAsync(new Tag()
                    {
                        DataCriacao = DateTime.Now,
                        NomeTag = nome.Trim(),
                        Tipo = tipo
                    });
                };

                Action<string> criarFormaPagamento = async (nome) =>
                {
                    await db.InsertAsync(new FormaPagamento()
                    {
                        DataCriacao = DateTime.Now,
                        Nome = nome.Trim()                        
                    });
                };

                foreach (var item in tagsReceita)
                {
                    criarTags(item, "+");
                }
                foreach (var item in tagsDespesa)
                {
                    criarTags(item, "-");
                }

                foreach (var item in formasPagamento)
                {
                    criarFormaPagamento(item);
                }

                //Cria o valor da versão
                await db.InsertAsync(new Configuracao() { Nome = "versao", Valor = "1", Sincronizado = false });
            }
            else
            {
                SQLiteAsyncConnection db = new SQLiteAsyncConnection(Constantes.DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, false);

                //Prepara para o processo de atualização
                var versaoDB = 0;
                var config = await db.Table<Configuracao>().Where(c => c.Nome == "versao").FirstOrDefaultAsync();
                if (config == null)
                {
                    await db.InsertAsync(new Configuracao() { Nome = "versao", Valor = "1", Sincronizado = false });
                }
                else
                    versaoDB = int.Parse(config.Valor);

                var updateCommand = new SQLiteConnection(Constantes.DatabasePath, SQLiteOpenFlags.ReadWrite).CreateCommand("", new object[] { });
                
                if(versaoDB == 0) //Não existia versão
                {
                    updateCommand.CommandText = @"alter table Lancamento Add `NumeroDocumento`	TEXT";
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
