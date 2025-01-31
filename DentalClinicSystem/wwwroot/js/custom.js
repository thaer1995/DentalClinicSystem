@section Scripts {
    <script>
        $(document).ready(function () {
            function updateTotals() {
                let grandTotal = 0;
                $('.treatment-item').each(function () {
                    const price = parseFloat($(this).find('.price').val()) || 0;
                    const quantity = parseInt($(this).find('.quantity').val()) || 0;
                    const total = price * quantity;
                    $(this).find('.total').val(total.toFixed(2));
                    grandTotal += total;
                });
                $('#totalAmount').text(grandTotal.toFixed(2));
            }

            $(document).on('change', '.treatment-select', function () {
                const price = $(this).find(':selected').data('price');
        $(this).closest('.treatment-item').find('.price').val(price);
        updateTotals();
            });

        $(document).on('change', '.quantity', function () {
            updateTotals();
            });

        $('#addTreatment').click(function () {
                const newItem = $('.treatment-item').first().clone();
        const index = $('.treatment-item').length;

        newItem.find('select').attr('name', `Items[${index}].TreatmentId`).val('');
        newItem.find('.quantity').attr('name', `Items[${index}].Quantity`).val(1);
        newItem.find('.price').attr('name', `Items[${index}].Price`).val('');
        newItem.find('.total').val('');

        $('#treatmentItems').append(newItem);
            });

        $(document).on('click', '.remove-item', function () {
                if ($('.treatment-item').length > 1) {
            $(this).closest('.treatment-item').remove();
        updateTotals();
                }
            });
        });
    </script>
}