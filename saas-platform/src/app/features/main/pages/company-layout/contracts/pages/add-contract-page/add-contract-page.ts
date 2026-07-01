import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RequestState } from '../../../../../../../core/models/request-state';
import { CompanyStore } from '../../../../../store/company.store';
import { AddContractDto } from '../../models/add-contract-dto';
import { Message } from '../../../../../../../shared/ui/message/message';

@Component({
  selector: 'app-add-contract-page',
  imports: [
    ReactiveFormsModule,
    DatePipe,
    Message
  ],
  templateUrl: './add-contract-page.html',
  styleUrl: './add-contract-page.scss',
})
export class AddContractPage {
  // DI
  readonly companyStore = inject(CompanyStore)

  actualDate = Date.now();
  request = this.companyStore.request.asReadonly()

  form = new FormGroup({
    title: new FormControl("", {nonNullable: true}),
    description: new FormControl("", {nonNullable: true}),
    price: new FormControl(0, {nonNullable: true}),
    deadline: new FormControl("", {nonNullable: true})
  })

  ngOnInit(): void {
    this.companyStore.clearRequestState();
  }

  addContract(){
    let deadline = this.form.controls.deadline.value;

    if (!deadline) {
      const date = new Date();
      date.setDate(date.getDate() + 30);

      deadline = date.toISOString();
    }

    const request: AddContractDto = {
      ...this.form.getRawValue(),
      deadline
    }
    this.companyStore.addContract(request)
    .subscribe()
  }

  onEnter(event: Event) {
    const textarea = event.target as HTMLTextAreaElement;

    const start = textarea.selectionStart;
    const value = textarea.value;

    const beforeCursor = value.substring(0, start);
    const lines = beforeCursor.split('\n');
    const currentLine = lines[lines.length - 1];

    const match = currentLine.match(/^(\d+)\.\s/);

    if (!match) {
      return;
    }

    event.preventDefault();

    const nextNumber = Number(match[1]) + 1;

    const insertText = `\n${nextNumber}. `;

    const newValue =
      value.substring(0, start) +
      insertText +
      value.substring(start);

    this.form.patchValue({
      description: newValue
    });

    setTimeout(() => {
      textarea.selectionStart =
        textarea.selectionEnd =
        start + insertText.length;
    });
  }
}
