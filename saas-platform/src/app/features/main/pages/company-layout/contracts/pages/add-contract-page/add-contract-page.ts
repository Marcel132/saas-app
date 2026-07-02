import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CompanyStore } from '../../../../../store/company.store';
import { AddContractDto } from '../../models/add-contract-dto';
import { Message } from '../../../../../../../shared/ui/message/message';
import { InfoTooltip } from "../../../../../../../shared/ui/info-tooltip/info-tooltip";

@Component({
  selector: 'app-add-contract-page',
  imports: [
    ReactiveFormsModule,
    DatePipe,
    Message,
    InfoTooltip
],
  templateUrl: './add-contract-page.html',
  styleUrl: './add-contract-page.scss',
})
export class AddContractPage {
  // DI
  readonly companyStore = inject(CompanyStore)

  actualDate = Date.now();
  request = this.companyStore.request.asReadonly()

  // TODO: ADD validation for UX exp


  form = new FormGroup({
    title: new FormControl("", { nonNullable: true }),
    description: new FormControl("", { nonNullable: true }),
    pricePerRequest: new FormControl(0, { nonNullable: true }),
    maxRequests: new FormControl(0, {
      validators: [
        Validators.required,
        Validators.min(1),
        Validators.pattern(/^\d+$/)
      ],
      nonNullable: true,
    }),
    deadline: new FormControl("", { nonNullable: true }),
  })

  ngOnInit(): void {
    this.companyStore.clearRequestState();
  }

  get maxBudget(): number {
    return (this.form.controls.pricePerRequest.value ?? 0) *
      (this.form.controls.maxRequests.value ?? 0);
  }

  addContract() {
    let deadline = this.form.controls.deadline.value;

    if (!deadline) {
      const date = new Date();
      date.setDate(date.getDate() + 30);

    }

    const request: AddContractDto = {
      ...this.form.getRawValue(),
      deadline
    }

    if(this.form.invalid)
      return;

    this.companyStore.addContract(request)
      .subscribe()
  }

  onEnter(event: Event) {
    console.log("enter")
    const textarea = event.target as HTMLTextAreaElement;

    const start = textarea.selectionStart;
    const value = textarea.value;

    const lineStart = value.lastIndexOf('\n', start - 1) + 1;
    const currentLine = value.slice(lineStart, start);

    const match = currentLine.match(/^(\d+)\./);

    if (!match) {
      return;
    }

    event.preventDefault();

    const nextNumber = Number(match[1]) + 1;

    const insertText = `\n${nextNumber}. `;

    const newValue = value.substring(0, start) + insertText + value.substring(start);

    console.log(newValue);

    this.form.patchValue({
      description: newValue
    });
    console.log(this.form.value.description);

    setTimeout(() => {
      textarea.selectionStart =
        textarea.selectionEnd =
        start + insertText.length;
    });
  }
}
