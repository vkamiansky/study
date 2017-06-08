namespace FsComposite

module CompositeToolset = 

    type 'a Composite =
    | Value of 'a
    | Composite of seq<Composite<'a>>
