using System;
using Microsoft.AspNetCore.Components;

namespace RclTest.Components
{
	public partial class Component1 : ComponentBase
	{
        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private async Task OnInput(ChangeEventArgs args)
        {
            Value = args.Value?.ToString() ?? string.Empty;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

    }
}
