function validateInput(input: HTMLInputElement, error: string): boolean {
  if (!input.value.trim()) {
    input.setCustomValidity(error);
    return false;
  } else {
    input.setCustomValidity('');
    return true;
  }
}

export function reportValidity(inputs: (HTMLInputElement | null)[]): void {
  for (let i = 0; i < inputs.length; i++) {
    if (!inputs[i]!.reportValidity()) {
      break;
    }
  }
}

export function validateInputs(inputs: {element: HTMLInputElement | null, error: string}[]): boolean {
    let validationPassed = true;

    inputs.forEach(input => {
      if (!validateInput(input.element!, input.error))
        validationPassed = false;
    });

    if (!validationPassed) {
      reportValidity(inputs.map(input => input.element!));
    }

    return validationPassed;
}
