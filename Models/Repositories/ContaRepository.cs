using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DinDinPro.Universal.Models.Repositories
{
    public enum TipoOrdenacaoLancamento
    {
        toDataDesc,
        toDataAsc,
        toDescription,
        toAmmountDesc,
        toAmmountAsc
    }

    public class ContaRepository : IContaRepository
    {
        private readonly IDataService _dataService;


        public ContaRepository(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<int> CriarContaAsync(string nome)
        {
            var obj = new Conta();
            obj.DataCriacao = DateTime.Now;
            obj.NomeConta = nome;
            obj.Sincronizado = false;

            await _dataService.Contexto.InsertAsync(obj);

#if DEBUG
            //Action<string, string, decimal, decimal, DateTime> criarLancamento = async (descricao, tipo, valor, valorRealizado, data) =>
            //    {
            //        Lancamento lancamento = new Lancamento();
            //        lancamento.ContaId = obj.ContaId;
            //        lancamento.DataCriacao = DateTime.Now;
            //        lancamento.DataLancamento = data;
            //        lancamento.Descricao = descricao;
            //        lancamento.Fechado = true;
            //        lancamento.Tipo = tipo;
            //        lancamento.TipoFimRepeticao = 0;
            //        lancamento.TipoRepeticao = 0;
            //        lancamento.ValorLancamento = valor;
            //        lancamento.ValorLancamentoRealizado = valorRealizado;
            //        await _dataService.Contexto.InsertAsync(lancamento);

            //        var tags = await _dataService.Contexto.Table<Tag>().Where(c => c.Tipo == tipo).ToListAsync();
            //        foreach (var item in tags)
            //        {
            //            LancamentoTag tag = new LancamentoTag();
            //            tag.LancamentoId = lancamento.LancamentoId;
            //            tag.DataCriacao = DateTime.Now;
            //            tag.Sincronizado = false;
            //            tag.TagId = item.TagId;
            //            await _dataService.Contexto.InsertAsync(tag);
            //        }

            //    };

            ////Cria alguns lançamentos de teste
            //criarLancamento("lançamento de testes", "+", 500, 500, DateTime.Now);
            //criarLancamento("lançamento de testes", "+", 1500, 1500, DateTime.Now);
            //criarLancamento("lançamento de testes", "+", 50, 50, DateTime.Now);
            ////criarLancamento("lançamento de testes", "+", 210, 210, DateTime.Now);
            ////criarLancamento("lançamento de testes", "+", 110, 110, DateTime.Now);
            ////criarLancamento("lançamento de testes", "+", 250, 350, DateTime.Now.AddMonths(1));
            ////criarLancamento("lançamento de testes", "-", 1500, 1859, DateTime.Now);
            ////criarLancamento("lançamento de testes", "-", 200, 200, DateTime.Now);
            ////criarLancamento("lançamento de testes", "+", 400, 350, DateTime.Now.AddMonths(1));

            //criarLancamento("lançamento de testes", "+", 110, 250, DateTime.Now.AddDays(-1));
            ////criarLancamento("lançamento de testes", "-", 188, 189.90M, DateTime.Now.AddDays(2));
            ////criarLancamento("lançamento de testes", "+", 1500, 1350.19M, DateTime.Now.AddMonths(-2));
            ////criarLancamento("lançamento de testes", "+", 500, 550, DateTime.Now.AddMonths(5));
            ////criarLancamento("lançamento de testes", "-", 178, 178, DateTime.Now.AddDays(-8));
            ////criarLancamento("lançamento de testes", "-", 200, 250, DateTime.Now.AddDays(1));
            ////criarLancamento("lançamento de testes", "-", 93, 102.90M, DateTime.Now);



#endif
            return obj.ContaId;
        }

        public async Task<bool> RemoverContaAsync(int contaId)
        {
            //Remove todos os lancamentos referente a conta
            await _dataService.Contexto.ExecuteAsync("delete from Lancamento where ContaId = ?", contaId);
            var obj = await _dataService.Contexto.GetAsync<Conta>(contaId);
            return await _dataService.Contexto.DeleteAsync(obj) > 0;
        }


        public async Task<IReadOnlyList<ContaView>> ListarContasAsync()
        {

            string sql = @"select c.ContaId, c.NomeConta
                        , (select ifnull(sum(l.ValorLancamentoRealizado),0) from Lancamento l where l.ContaId = c.ContaId and l.Fechado = 1 AND l.Tipo = '+' AND date(l.DataLancamento) <= date('now')) -
                          (select ifnull(sum(l.ValorLancamentoRealizado),0) from Lancamento l where l.ContaId = c.ContaId and l.Fechado = 1 AND l.Tipo = '-' AND date(l.DataLancamento) <= date('now')) SaldoRealizado 
                        , (select ifnull(sum(l.ValorLancamento),0) from Lancamento l where l.ContaId = c.ContaId and l.Fechado = 0 AND l.Tipo = '+' AND date(l.DataLancamento) <= date('now')) -
                          (select ifnull(sum(l.ValorLancamento),0) from Lancamento l where l.ContaId = c.ContaId and l.Fechado = 0 AND l.Tipo = '-' AND date(l.DataLancamento) <= date('now')) SaldoPrevisto
                        from Conta c ";
            return await _dataService.Contexto.QueryAsync<ContaView>(sql, new object[] { });
        }

        public async Task<decimal> ObterSaldoAtual()
        {
            string sql1 = @"select (select ifnull(SUM(l.ValorLancamentoRealizado),0) from Lancamento l WHERE date(l.DataLancamento) <= date('now') AND l.Fechado = 1 AND l.Tipo = '+')  -
                                    (select ifnull(SUM(l.ValorLancamentoRealizado),0) from Lancamento l WHERE date(l.DataLancamento) <= date('now') AND l.Fechado = 1 AND l.Tipo = '-' ) saldo";


            var saldoRealizado = await _dataService.Contexto.ExecuteScalarAsync<decimal>(sql1, new object[] { });
            return saldoRealizado;
        }

        public Task<decimal> ObterSaldoPrevistoAtual()
        {
            throw new NotImplementedException();
        }


        public Task<Conta> ObterDetalhes(int contaId)
        {
            var retorno = _dataService.Contexto.FindAsync<Conta>(contaId);
            return retorno;
        }

        private async Task<IList<LancamentoView>> ObterLancamentos(int? lancamentoId)
        {
            return await ObterLancamentos(null, lancamentoId, null, TipoOrdenacaoLancamento.toDataDesc);
        }

        public async Task<IList<LancamentoView>> ObterLancamentos(DateTime periodo)
        {
            return await ObterLancamentos(null, null, periodo, TipoOrdenacaoLancamento.toDataDesc);
        }


        public async Task<IList<LancamentoView>> ObterLancamentos(int? contaId, int? lancamentoId, DateTime? periodo, TipoOrdenacaoLancamento ordenacao = TipoOrdenacaoLancamento.toDataDesc)
        {
            var sqlLancamentos = @"select l.lancamentoid,l.DataCriacao,l.DataAlteracao,l.ValorLancamento,l.ValorLancamentoRealizado,
                                    l.DataLancamento, l.Descricao,l.ContaId,l.TipoRepeticao, l.TipoFimRepeticao,l.FimRepeticaoData,
                                    l.FimRepeticaoQuantidade,l.Tipo,l.LancamentoPaiId,l.Compartilhar,l.Sincronizado,l.Fechado,l.ReceberAviso,
                                    l.FormaPagamentoId, c.NomeConta, f.Nome as FormaPagamento
                                    from Lancamento l 
                                    inner join Conta c on c.ContaId = l.ContaId
                                    left join FormaPagamento f on l.FormaPagamentoId = f.FormaPagamentoId
                                  where 1=1 ";

            var parametros = new List<object>();
            if (periodo != null)
            {
                sqlLancamentos += " and Date(l.DataLancamento) >= ? and Date(l.DataLancamento) <= ?";
                var data1 = new DateTime(periodo.Value.Year, periodo.Value.Month, 1);
                var data2 = new DateTime(periodo.Value.Year, periodo.Value.Month, DateTime.DaysInMonth(periodo.Value.Year, periodo.Value.Month));

                parametros.Add(data1.ToString("yyyy-MM-dd"));
                parametros.Add(data2.ToString("yyyy-MM-dd"));

                //parametros.Add(data1.Ticks);
                //parametros.Add(data2.Ticks);
            }



            if (contaId != null)
            {
                sqlLancamentos += " and l.ContaId = ? ";
                parametros.Add(contaId);
            }

            if (lancamentoId != null)
            {
                sqlLancamentos += " and l.LancamentoId = ? ";
                parametros.Add(lancamentoId);
            }

            if (ordenacao == TipoOrdenacaoLancamento.toDataAsc)
                sqlLancamentos += " order by l.DataLancamento ASC";
            else if (ordenacao == TipoOrdenacaoLancamento.toDataDesc)
                sqlLancamentos += " order by l.DataLancamento DESC";
            else if (ordenacao == TipoOrdenacaoLancamento.toAmmountAsc)
                sqlLancamentos += " order by l.ValorLancamento ASC";
            else if (ordenacao == TipoOrdenacaoLancamento.toAmmountDesc)
                sqlLancamentos += " order by l.ValorLancamento DESC";
            

            List<LancamentoView> retorno = new List<LancamentoView>();
            var lancamentos = await _dataService.Contexto.QueryAsync<LancamentoView>(sqlLancamentos, parametros.ToArray());


            Func<List<LancamentoTagView>, string> obterTags = (lista) =>
                {
                    StringBuilder saida = new StringBuilder();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        saida.Append(lista[i].NomeTag + (i < lista.Count - 1 ? "," : String.Empty));
                    }
                    return saida.ToString();
                };

            var sql = @"select l.LancamentoId, l.TagId, l.DataCriacao, l.Sincronizado, t.NomeTag
                            from LancamentoTag l
                            left join Tag t on t.TagId = l.TagId
                            where LancamentoId = ?";
            foreach (var item in lancamentos)
            {

                var tags = await _dataService.Contexto.QueryAsync<LancamentoTagView>(sql, new object[] { item.LancamentoId });
                item.Tags = obterTags(tags);

                retorno.Add(item);
            }
            if (ordenacao == TipoOrdenacaoLancamento.toDescription)
                return retorno.OrderBy(c => c.Tags).ToList();

            return retorno;
        }


        public async Task<bool> CriarLancamentoAsync(LancamentoView lancamento)
        {
            var inserindo = lancamento.LancamentoId < 1;

            List<int> tagsProcessadas = new List<int>();
            var result = false;


            var objInsert = new Lancamento();
            foreach (PropertyInfo item in lancamento.GetType().GetRuntimeProperties())
            {
                var propToFill = objInsert.GetType().GetRuntimeProperty(item.Name);
                if (propToFill != null)
                    propToFill.SetValue(objInsert, item.GetValue(lancamento));
            }

            //Verifica se trata de uma repetição
            if (objInsert.TipoRepeticao > 0)
            {
                objInsert.Parcela = 1;
                lancamento.Parcela = 1;
            }

            if (lancamento.LancamentoId > 0)
                result = await _dataService.Contexto.UpdateAsync(objInsert) > 0;
            else
                result = await _dataService.Contexto.InsertAsync(objInsert) > 0;

            if (!inserindo)
            {
                //Apaga todos os lançamento tags e lancamentos filhos (se houver)
                await _dataService.Contexto.ExecuteAsync("delete from LancamentoTag where LancamentoId = ?", lancamento.LancamentoId);

                //Remove os filhos (caso tenha)
                await _dataService.Contexto.ExecuteAsync("delete from Lancamento where LancamentoPaiId = ? and ContaId = ?", lancamento.LancamentoId, lancamento.ContaId);

                //Apaga os irmaos
                await _dataService.Contexto.ExecuteAsync("delete from Lancamento where LancamentoPaiId = ? and ContaId = ?", lancamento.LancamentoPaiId, lancamento.ContaId);
            }

            //Tivemos sucesso, processa as tags
            if (result)
            {
                foreach (var tag in lancamento.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //Tenta localizar a tag
                    var obj = await _dataService.Contexto.Table<Tag>().Where(c => c.NomeTag.ToLower() == tag.ToLower()).FirstOrDefaultAsync();

                    if (obj != null)
                    {
                        //Cria o lançamento tag
                        LancamentoTag lancamentoTag = new LancamentoTag()
                        {
                            DataCriacao = DateTime.Now,
                            LancamentoId = objInsert.LancamentoId,
                            TagId = obj.TagId
                        };
                        await _dataService.Contexto.InsertAsync(lancamentoTag);
                        tagsProcessadas.Add(lancamentoTag.TagId);
                    }
                    else
                    {
                        //Se não achar a tag, cria 
                        Tag novaTag = new Tag()
                        {
                            DataCriacao = DateTime.Now,
                            NomeTag = tag,
                            Tipo = lancamento.Tipo
                        };
                        if (await _dataService.Contexto.InsertAsync(novaTag) > 0)
                        {
                            LancamentoTag novoLancamentoTag = new LancamentoTag()
                            {
                                DataCriacao = DateTime.Now,
                                LancamentoId = objInsert.LancamentoId,
                                TagId = novaTag.TagId
                            };
                            await _dataService.Contexto.InsertAsync(novoLancamentoTag);
                            tagsProcessadas.Add(novoLancamentoTag.TagId);
                        }
                    }
                }

                if (lancamento.TipoRepeticao > 0)
                {
                     await ProcessarRepeticao2(objInsert, tagsProcessadas);
                }
            }
            return result;
        }

        private async Task<bool> ProcessarRepeticao2(Lancamento lancamentoBase, List<int> tagsProcessadas)
        {
            var parcela = 2; //Se estamos passando aqui, trata-se de uma repetição, certo?
            var result = false;
            IList<Lancamento> lista = new List<Lancamento>();
            Func<Lancamento,  DateTime?, bool> criarLancamento = (lancamento, data) =>
            {
                var lancamentoRepeticao = new Lancamento()
                {
                    DataCriacao = lancamento.DataCriacao,
                    ContaId = lancamento.ContaId,
                    DataLancamento = data,
                    Descricao = lancamento.Descricao,
                    FimRepeticaoData = lancamento.FimRepeticaoData,
                    FimRepeticaoQuantidade = lancamento.FimRepeticaoQuantidade,
                    LancamentoPaiId = lancamento.LancamentoId,
                    Tipo = lancamento.Tipo,
                    TipoFimRepeticao = lancamento.TipoFimRepeticao,
                    TipoRepeticao = lancamento.TipoRepeticao,
                    ValorLancamento = lancamento.ValorLancamento,
                    ValorLancamentoRealizado = lancamento.ValorLancamentoRealizado,
                    Fechado = lancamento.Fechado,
                    FormaPagamentoId = lancamento.FormaPagamentoId,
                    Parcela = parcela

                };

                //await _dataService.Contexto.InsertAsync(lancamentoRepeticao);
                lista.Add(lancamentoRepeticao);

                foreach (var item in tagsProcessadas)
                {

                    LancamentoTag novoLancamentoTag = new LancamentoTag()
                    {
                        DataCriacao = DateTime.Now,
                        LancamentoId = lancamentoRepeticao.LancamentoId,
                        TagId = item
                    };

                    lancamentoRepeticao.TagsSelecionadas.Add(novoLancamentoTag);
                    //await _dataService.Contexto.InsertAsync(novoLancamentoTag);
                }
                parcela++;
                return true;
            };
            
            var proximaData = lancamentoBase.DataLancamento;
            var incremento = 0;
            switch (lancamentoBase.TipoRepeticao)
            {
                case 1:
                    incremento = 1;
                    break;
                case 2:
                    incremento = 7;
                    break;
                case 3:
                    incremento = 15;
                    break;
                case 4:
                    incremento = 30;
                    break;
                case 5:
                    incremento = 365;                    
                    break;
            }

            var quantidade = 1;
            do
            {
                
                var diasMes = DateTime.DaysInMonth(proximaData.Value.Year, proximaData.Value.Month);
                if (incremento == 30) //Mensal, pega a quantidade de dias do mês atual
                    proximaData = proximaData.Value.AddDays(diasMes);
                else
                    proximaData = proximaData.Value.AddDays(incremento);

                
                //Tipo de repetição (dias uteis e finais de semana), tem uma checagem adicional
                if (lancamentoBase.TipoFimRepeticao == 6 || lancamentoBase.TipoRepeticao == 7)
                {
                    var condicaoValida = lancamentoBase.TipoRepeticao == 6 ? proximaData.Value.DayOfWeek != DayOfWeek.Saturday && proximaData.Value.DayOfWeek != DayOfWeek.Sunday
                                        : proximaData.Value.DayOfWeek == DayOfWeek.Saturday || proximaData.Value.DayOfWeek == DayOfWeek.Sunday;

                    if (condicaoValida)
                    {
                        criarLancamento(lancamentoBase, proximaData);
                    }
                }
                else
                    criarLancamento(lancamentoBase, proximaData);

                if (lancamentoBase.TipoFimRepeticao == 1) //Data{
                {

                    if (proximaData > lancamentoBase.FimRepeticaoData)
                        break;
                }
                else
                {
                    if (quantidade >= lancamentoBase.FimRepeticaoQuantidade.Value -1)
                        break;
                    quantidade++;
                }
            } while (true);

            
            //Salva os lancamentos
            if (await _dataService.Contexto.InsertAllAsync(lista) > 0)
            {
                var tempTag = new HashSet<LancamentoTag>();
                foreach (var item in lista)
                {
                    foreach (var tag in item.TagsSelecionadas)
                    {
                        tag.LancamentoId = item.LancamentoId;
                        tempTag.Add(tag);
                    }

                }

                //Processa as tags
                await _dataService.Contexto.InsertAllAsync(tempTag);
            }

            return result;
        }

        private async Task<bool> ProcessarRepeticao(Lancamento lancamentoBase, List<int> tagsProcessadas)
        {
            var parcela = 2; //Se estamos passando aqui, trata-se de uma repetição, certo?
            var result = false;
            IList<Lancamento> lista = new List<Lancamento>();
            Func<Lancamento, int, DateTime?, bool> criarLancamento =  (lancamento, incremento, data) =>
            {
                var lancamentoRepeticao = new Lancamento()
                {
                    DataCriacao = lancamento.DataCriacao,
                    ContaId = lancamento.ContaId,
                    DataLancamento = data ?? lancamento.DataLancamento.Value.AddDays(incremento),
                    Descricao = lancamento.Descricao,
                    FimRepeticaoData = lancamento.FimRepeticaoData,
                    FimRepeticaoQuantidade = lancamento.FimRepeticaoQuantidade,
                    LancamentoPaiId = lancamento.LancamentoId,
                    Tipo = lancamento.Tipo,
                    TipoFimRepeticao = lancamento.TipoFimRepeticao,
                    TipoRepeticao = lancamento.TipoRepeticao,
                    ValorLancamento = lancamento.ValorLancamento,
                    ValorLancamentoRealizado = lancamento.ValorLancamentoRealizado,
                    Fechado = lancamento.Fechado,
                    FormaPagamentoId = lancamento.FormaPagamentoId,
                    Parcela = parcela

                };

                //await _dataService.Contexto.InsertAsync(lancamentoRepeticao);
                lista.Add(lancamentoRepeticao);

                foreach (var item in tagsProcessadas)
                {
                    LancamentoTag novoLancamentoTag = new LancamentoTag()
                    {
                        DataCriacao = DateTime.Now,
                        LancamentoId = lancamentoRepeticao.LancamentoId,
                        TagId = item
                    };

                    lancamentoRepeticao.TagsSelecionadas.Add(novoLancamentoTag);
                    //await _dataService.Contexto.InsertAsync(novoLancamentoTag);
                }
                parcela++;
                return true;
            };

            var contador = 1;
            switch (lancamentoBase.TipoFimRepeticao)
            {
                case 1: //Data
                    var dataBaseContagem = lancamentoBase.DataLancamento;
                    var dias = lancamentoBase.FimRepeticaoData.Value.Subtract(lancamentoBase.DataLancamento.Value).Days;
                    for (int i = 0; i <= dias; i++)
                    {
                        var diasIncremento = 0;
                        //Never repeat,Daily,Weekly,Fortnightly,Monthly,Annual,Weekdays,Weekends
                        switch (lancamentoBase.TipoRepeticao)
                        {
                            case 1:
                                diasIncremento = 1;
                                break;
                            case 2:
                                diasIncremento = 7;
                                break;
                            case 3:
                                diasIncremento = 15;
                                break;
                            case 4:
                                diasIncremento = DateTime.DaysInMonth(dataBaseContagem.Value.Year, dataBaseContagem.Value.Month);
                                break;
                            case 5:
                                diasIncremento = 365;
                                break;
                            default:
                                diasIncremento = 1;
                                break;
                        }



                        //Se atingir a database, para tudo
                        if (dataBaseContagem > lancamentoBase.FimRepeticaoData)
                            break;

                        if (lancamentoBase.TipoRepeticao == 1)
                            criarLancamento(lancamentoBase, i + 1, null);
                        else if (lancamentoBase.TipoRepeticao == 2)
                            criarLancamento(lancamentoBase, (i + 1) * 7, null);
                        else if (lancamentoBase.TipoRepeticao == 3)
                            criarLancamento(lancamentoBase, (i + 1) * 15, null);
                        else if (lancamentoBase.TipoRepeticao == 4)
                            criarLancamento(lancamentoBase, (i + 1) * 30, null);
                        else if (lancamentoBase.TipoRepeticao == 5)
                            criarLancamento(lancamentoBase, (i + 1) * 365, null);
                        else if (lancamentoBase.TipoRepeticao == 6 || lancamentoBase.TipoRepeticao == 7) //Dias úteis e finais de semana, o tratamento é um pouco diferente
                        {
                            var condicaoValida = lancamentoBase.TipoRepeticao == 6 ? dataBaseContagem.Value.DayOfWeek != DayOfWeek.Saturday && dataBaseContagem.Value.DayOfWeek != DayOfWeek.Sunday
                                     : dataBaseContagem.Value.DayOfWeek == DayOfWeek.Saturday || dataBaseContagem.Value.DayOfWeek == DayOfWeek.Sunday;

                            if (condicaoValida)
                            {
                                criarLancamento(lancamentoBase, 0, dataBaseContagem);
                                contador++;
                            }
                        }
                        dataBaseContagem = dataBaseContagem.Value.AddDays(diasIncremento);
                    }
                    break;
                case 2:  //Quantidade de repetições 
                    //Cria os lançamentos baseado na configuração de repetição
                    for (int i = 1; i < lancamentoBase.FimRepeticaoQuantidade; i++)
                    {
                        if (lancamentoBase.TipoRepeticao == 1)
                            criarLancamento(lancamentoBase, (i + 1), null);
                        else if (lancamentoBase.TipoRepeticao == 2)
                            criarLancamento(lancamentoBase, (i + 1) * 7, null);
                        else if (lancamentoBase.TipoRepeticao == 3)
                            criarLancamento(lancamentoBase, (i + 1) * 15, null);
                        else if (lancamentoBase.TipoRepeticao == 4)
                            criarLancamento(lancamentoBase, (i + 1) * 30, null);
                        else if (lancamentoBase.TipoRepeticao == 5)
                            criarLancamento(lancamentoBase, (i + 1) * 365, null);
                        else if (lancamentoBase.TipoRepeticao == 6 || lancamentoBase.TipoRepeticao == 7) //Dias úteis e finais de semana, o tratamento é um pouco diferente
                        {
                            var dataBase = lancamentoBase.DataLancamento;
                            do
                            {
                                var condicaoValida = lancamentoBase.TipoRepeticao == 6 ? dataBase.Value.DayOfWeek != DayOfWeek.Saturday && dataBase.Value.DayOfWeek != DayOfWeek.Sunday
                                    : dataBase.Value.DayOfWeek == DayOfWeek.Saturday || dataBase.Value.DayOfWeek == DayOfWeek.Sunday;

                                if (condicaoValida)
                                {
                                    criarLancamento(lancamentoBase, 0, dataBase);
                                    contador++;
                                }
                                dataBase = dataBase.Value.AddDays(1);
                            } while (contador <= lancamentoBase.FimRepeticaoQuantidade);
                        }
                    }
                    break;
            }

            //Salva os lancamentos
            if(await _dataService.Contexto.InsertAllAsync(lista) > 0)
            {
                var tempTag = new HashSet<LancamentoTag>();
                foreach (var item in lista)
                {
                    foreach (var tag in item.TagsSelecionadas)
                    {
                        tag.LancamentoId = item.LancamentoId;
                        tempTag.Add(tag);
                    }
                    
                }

                //Processa as tags
                await _dataService.Contexto.InsertAllAsync(tempTag);
            }

            return result;
        }


        public async Task<IReadOnlyList<TagView>> ListarTagsAsync(string tipoLancamento)
        {
            var sql = @"select TagId, DataCriacao, DataAlteracao, NomeTag, Tipo, 0 as Selecionado from Tag where Tipo = ?";
            return await _dataService.Contexto.QueryAsync<TagView>(sql, tipoLancamento);
        }


        public async Task<decimal> ObterDespesasFuturas()
        {
            string sql = @"select ifnull(SUM(coalesce(l.ValorLancamentoRealizado,0)),0) Saldo from Lancamento l 
                            WHERE date(l.DataLancamento) > date('now') AND l.Fechado = 1 AND l.Tipo = '-' ";

            return await _dataService.Contexto.ExecuteScalarAsync<decimal>(sql, new object[] { });
        }


        public async Task<bool> RemoverLancamentoAsync(int lancamentoId)
        {
            return await _dataService.Contexto.ExecuteAsync("delete from Lancamento where LancamentoId = ?", lancamentoId) > 0;
        }


        public async Task<IReadOnlyList<GraficoDespesa>> ListarDespesasAgrupadas(DateTime periodo)
        {
            string sql = @"
                        SELECT t.NomeTag Despesa,
                               SUM(l.ValorLancamentoRealizado) Valor
                        FROM LancamentoTag lt
                        INNER JOIN Tag t
                                ON t.TagId = lt.TagId
                        INNER JOIN Lancamento l
                                ON l.LancamentoId = lt.LancamentoId
                        WHERE DATE(l.DataLancamento) BETWEEN DATE(?) AND (?)
                          AND l.Fechado = 1
                          AND l.Tipo = '-'
                        GROUP BY t.NomeTag";

            var data1 = new DateTime(periodo.Year, periodo.Month, 1);
            var data2 = new DateTime(periodo.Year, periodo.Month, DateTime.DaysInMonth(periodo.Year, periodo.Month));

            var parametros = new List<object>();

            parametros.Add(data1.ToString("yyyy-MM-dd"));
            parametros.Add(data2.ToString("yyyy-MM-dd"));

            return await _dataService.Contexto.QueryAsync<GraficoDespesa>(sql, parametros.ToArray());
        }

        public async Task<LancamentoView> ObterLancamento(int lancamentoId)
        {
            var lancamento = (await ObterLancamentos(lancamentoId)).FirstOrDefault();
            return lancamento;
        }

        public async Task<IList<PrevisaoInfo>> ListarPrevisao(DateTime dataBase)
        {
            String sql = @"select strftime('%Y-%m', DataLancamento) Data, Sum(ValorLancamento) Valor from Lancamento
                            where date(DataLancamento) BETWEEN ? AND ? AND Tipo = '-'
                            group by 1
                           union
                            select strftime('%Y-%m', date('now','+10 year')) Data, Sum(ValorLancamento) Valor from Lancamento
                            where date(DataLancamento) > ? AND Tipo = '-'
                            group by 1";

            var parametros = new List<object>();

            parametros.Add(dataBase.AddMonths(1).ToString("yyyy-MM"));
            parametros.Add(dataBase.AddMonths(4).ToString("yyyy-MM"));
            parametros.Add(dataBase.AddMonths(4).ToString("yyyy-MM"));
            var result = await _dataService.Contexto.QueryAsync<PrevisaoInfo>(sql, parametros.ToArray());

            //Pega as proximas            
            return  result;
        }
    }

    public class SaldoView
    {
        public decimal Saldo
        {
            get;
            set;
        }
    }
}
