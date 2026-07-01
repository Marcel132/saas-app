import { Component, effect, inject, signal } from '@angular/core';
import { CompanyStore } from '../../../../../store/company.store';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RequestState } from '../../../../../../../core/models/request-state';
import { Message } from "../../../../../../../shared/ui/message/message";

@Component({
  selector: 'app-edit-contract-page',
  imports: [
    ReactiveFormsModule,
    Message
],
  templateUrl: './edit-contract-page.html',
  styleUrl: './edit-contract-page.scss',
})
export class EditContractPage {
  // DI
  private readonly companyStore = inject(CompanyStore)
  private readonly route = inject(ActivatedRoute)

  request = this.companyStore.request.asReadonly()

  readonly contract = this.companyStore.selectedContract.asReadonly()
  private id!: number

  form = new FormGroup({
    title: new FormControl("", {nonNullable: true}),
    description: new FormControl("", {nonNullable: true}),
    price: new FormControl(),
    newDeadline: new FormControl()
  })

  constructor(){
    effect(() => {
    const contract = this.contract()

    if(!contract)
      return

    this.form.patchValue({
      title: contract.title,
      description: contract.description,
      price: contract.price,
      newDeadline: contract.deadline.split('T')[0]
    })
    this.form.markAsPristine();
    })
  }

  ngOnInit(): void {
    this.companyStore.clearRequestState();
    const id = Number(
      this.route.snapshot.paramMap.get('id')
    )
    if(Number.isNaN(id))
      return

    this.id = id;
    this.companyStore.getContractById(id)
      .subscribe()
  }

  update(){
    console.log(this.form.dirty);

    if(!this.form.dirty){
      return;
    }

    this.companyStore.updateContract(this.id, this.form.getRawValue())
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
