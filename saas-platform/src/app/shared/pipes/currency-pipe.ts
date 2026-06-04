import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'plnCurrency',
})
export class CurrencyPipe implements PipeTransform {
  transform(value: number | null | undefined): string {
    if(value == null)
      return '';

    return value.toLocaleString('pl-PL', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }) + ' zł';
  }
}
