using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface IInvoker
{
    protected SortedList<float, ICommand> Commands { get; set; }

    public void ExecuteCommand(ICommand command);

    public void AddCommand(ICommand command);
}
