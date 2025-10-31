using System;
using System.Drawing;
using System.Collections.Generic;

namespace EtiquetaFORNew
{
    // Classe de Produto
    public class Produto
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }

    // Enum de tipos de elementos
    public enum TipoElemento
    {
        Texto,
        Campo,
        CodigoBarras,
        Imagem
    }

    // Classe de Elemento da Etiqueta
    public class ElementoEtiqueta
    {
        public TipoElemento Tipo { get; set; }
        public string Conteudo { get; set; } // Para texto fixo ou nome do campo
        public Rectangle Bounds { get; set; }
        public Font Fonte { get; set; }
        public Color Cor { get; set; }
        public Image Imagem { get; set; }
        public bool Negrito { get; set; }
        public bool Italico { get; set; }

        public ElementoEtiqueta()
        {
            Fonte = new Font("Arial", 10);
            Cor = Color.Black;
        }
    }

    // Classe do Template de Etiqueta
    public class TemplateEtiqueta
    {
        public float Largura { get; set; } = 50; // mm
        public float Altura { get; set; } = 30; // mm
        public List<ElementoEtiqueta> Elementos { get; set; } = new List<ElementoEtiqueta>();
    }
}