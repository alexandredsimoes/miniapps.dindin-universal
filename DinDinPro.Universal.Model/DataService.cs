using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace DinDinPro.Universal.Models
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection _conexao;


        public DataService()
        {
           // _conexao = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), new SQLiteConnectionString(Constantes.DatabasePath, false)));
            var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), new SQLiteConnectionString(Constantes.DatabasePath, storeDateTimeAsTicks: false)));
            _conexao = new SQLiteAsyncConnection(connectionFactory);
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
                //SQLiteAsyncConnection db = new SQLiteAsyncConnection(Constantes.DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, false);

                await _conexao.CreateTableAsync<Configuracao>();
                await _conexao.CreateTableAsync<Conta>();
                await _conexao.CreateTableAsync<Lancamento>();
                await _conexao.CreateTableAsync<LancamentoTag>();
                await _conexao.CreateTableAsync<Tag>();
                await _conexao.CreateTableAsync<FormaPagamento>();
                
                //Cria o valor da versão
                await _conexao.InsertAsync(new Configuracao() { Nome = "versao", Valor = "1", Sincronizado = false });
      
                foreach (var item in tagsReceita)
                {
                    await _conexao.InsertAsync(new Tag()
                    {
                        DataCriacao = DateTime.Now,
                        NomeTag = item,
                        Tipo = "+"
                    });
                }
                foreach (var item in tagsDespesa)
                {
                    await _conexao.InsertAsync(new Tag()
                    {
                        DataCriacao = DateTime.Now,
                        NomeTag = item,
                        Tipo = "-"
                    });
                }

                foreach (var item in formasPagamento)
                {
                    await _conexao.InsertAsync(new FormaPagamento()
                    {
                        DataCriacao = DateTime.Now,
                        Nome = item.Trim()
                    });
                }

                
            }
            else
            {
                //SQLiteAsyncConnection db = new SQLiteAsyncConnection(Constantes.DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, false);

                //Prepara para o processo de atualização
                var versaoDB = 0;
                var config = await _conexao.Table<Configuracao>().Where(c => c.Nome == "versao").FirstOrDefaultAsync();
                if (config == null)
                {
                    await _conexao.InsertAsync(new Configuracao() { Nome = "versao", Valor = "1", Sincronizado = false });
                }
                else
                    versaoDB = int.Parse(config.Valor);


                
                //var updateCommand = _conexao.CreateCommand("", new object[] { });
                
                //if(versaoDB == 0) //Não existia versão
                //{
                //    updateCommand.CommandText = @"alter table Lancamento Add `NumeroDocumento`	TEXT";
                //    updateCommand.ExecuteNonQuery();
                //}
            }
        }
    }
}
