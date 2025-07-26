using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Mapeadores;

public static class ProfessorMapper
{
    public static Professor ToDomain(ProfessorEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Create professor using reflection to set private fields
        var professor = (Professor)Activator.CreateInstance(typeof(Professor), true)!;
        
        // Set basic properties using reflection
        var idProperty = typeof(Professor).GetProperty("Id");
        idProperty?.SetValue(professor, entity.Id);

        var nomeProperty = typeof(Professor).GetProperty("Nome");
        nomeProperty?.SetValue(professor, new NomeProfessor(entity.Nome));

        var cpfProperty = typeof(Professor).GetProperty("Cpf");
        cpfProperty?.SetValue(professor, new Cpf(entity.Cpf));

        var registroProperty = typeof(Professor).GetProperty("Registro");
        registroProperty?.SetValue(professor, new RegistroProfessor(entity.Registro));

        var emailProperty = typeof(Professor).GetProperty("Email");
        emailProperty?.SetValue(professor, entity.Email);

        var telefoneProperty = typeof(Professor).GetProperty("Telefone");
        telefoneProperty?.SetValue(professor, entity.Telefone);

        var dataNascimentoProperty = typeof(Professor).GetProperty("DataNascimento");
        dataNascimentoProperty?.SetValue(professor, entity.DataNascimento);

        var dataContratacaoProperty = typeof(Professor).GetProperty("DataContratacao");
        dataContratacaoProperty?.SetValue(professor, entity.DataContratacao);

        var escolaIdProperty = typeof(Professor).GetProperty("EscolaId");
        escolaIdProperty?.SetValue(professor, entity.EscolaId);

        var ativoProperty = typeof(Professor).GetProperty("Ativo");
        ativoProperty?.SetValue(professor, entity.Ativo);

        var dataCadastroProperty = typeof(Professor).GetProperty("DataCadastro");
        dataCadastroProperty?.SetValue(professor, entity.DataCadastro);

        var observacoesProperty = typeof(Professor).GetProperty("Observacoes");
        observacoesProperty?.SetValue(professor, entity.Observacoes);

        // Set base entity properties
        var createdAtProperty = typeof(Professor).GetProperty("CreatedAt");
        createdAtProperty?.SetValue(professor, entity.DataCadastro);

        var updatedAtProperty = typeof(Professor).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(professor, entity.UpdatedAt);

        // Add titles
        if (entity.Titulos?.Any() == true)
        {
            var titulosField = typeof(Professor).GetField("_titulos", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (titulosField != null)
            {
                var titulosList = (List<TituloAcademico>)titulosField.GetValue(professor)!;
                titulosList.Clear();
                
                foreach (var tituloEntity in entity.Titulos)
                {
                    var titulo = new TituloAcademico(
                        (TipoTitulo)tituloEntity.Tipo,
                        tituloEntity.Curso,
                        tituloEntity.Instituicao,
                        tituloEntity.AnoFormatura);
                    
                    titulosList.Add(titulo);
                }
            }
        }

        // Add disciplines
        if (entity.Disciplinas?.Any() == true)
        {
            var disciplinasField = typeof(Professor).GetField("_disciplinas", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (disciplinasField != null)
            {
                var disciplinasList = (List<ProfessorDisciplina>)disciplinasField.GetValue(professor)!;
                disciplinasList.Clear();
                
                foreach (var disciplinaEntity in entity.Disciplinas)
                {
                    var disciplina = ProfessorDisciplinaMapper.ToDomain(disciplinaEntity);
                    disciplinasList.Add(disciplina);
                }
            }
        }

        return professor;
    }

    public static ProfessorEntity ToEntity(Professor professor)
    {
        if (professor == null)
            throw new ArgumentNullException(nameof(professor));

        var entity = new ProfessorEntity
        {
            Id = professor.Id,
            Nome = professor.Nome.Valor,
            Cpf = professor.Cpf.Numero,
            Registro = professor.Registro.Numero,
            Email = professor.Email,
            Telefone = professor.Telefone,
            DataNascimento = professor.DataNascimento,
            DataContratacao = professor.DataContratacao,
            EscolaId = professor.EscolaId,
            Ativo = professor.Ativo,
            DataCadastro = professor.DataCadastro,
            UpdatedAt = professor.UpdatedAt,
            Observacoes = professor.Observacoes
        };

        // Map titles
        entity.Titulos = professor.Titulos.Select(titulo => new TituloAcademicoEntity
        {
            Id = Guid.NewGuid(),
            ProfessorId = professor.Id,
            Tipo = (int)titulo.Tipo,
            Curso = titulo.Curso,
            Instituicao = titulo.Instituicao,
            AnoFormatura = titulo.AnoFormatura,
            DataCadastro = DateTime.UtcNow
        }).ToList();

        // Map disciplines
        entity.Disciplinas = professor.Disciplinas.Select(disciplina => 
            ProfessorDisciplinaMapper.ToEntity(disciplina)).ToList();

        return entity;
    }

    public static void UpdateEntity(ProfessorEntity entity, Professor professor)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        if (professor == null)
            throw new ArgumentNullException(nameof(professor));

        entity.Nome = professor.Nome.Valor;
        entity.Email = professor.Email;
        entity.Telefone = professor.Telefone;
        entity.Ativo = professor.Ativo;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Observacoes = professor.Observacoes;
        entity.EscolaId = professor.EscolaId;
    }
}