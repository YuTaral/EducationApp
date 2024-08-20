const NUMERIC = "Numeric";
const TEXT = "Text";
const MULTIPLE_ANSWER = "Multiple answer";
const SINGLE_ANSWER = "Single answer";

function populateSelectableQuestions() {
    var question = document.querySelectorAll('.selectable-question')[0];

    if (question != undefined) {
        var typeAndPossibleAns = question.querySelectorAll('input');
        var type = typeAndPossibleAns[0];

        var answersContainers = document.querySelectorAll('.answers-container')[0];
        var possibleAnswers = typeAndPossibleAns[1].value.split('|');
        var answerOptionId = type.value == SINGLE_ANSWER ? 'answerOptionSingle' : 'answerOptionMulti';
        var answerOption = document.getElementById(answerOptionId);

        for (j = 0; j < possibleAnswers.length; j++) {
            var cloned = answerOption.cloneNode(true);
            var span = cloned.querySelectorAll('.answer-text')[0];
            var answerId = cloned.querySelectorAll('.answer-id')[0];
            var correctOrNo = cloned.querySelectorAll('.correct-or-no')[0];
            var splittedData = possibleAnswers[j].split("_");
            var idAndIsCorrect = splittedData[0].split("#");
            var id = idAndIsCorrect[0];
            var isCorect = idAndIsCorrect[1];
            var text = splittedData[1];

            answerId.value = id;
            span.innerText = text;

            if (isCorect == 'True') {
                correctOrNo.innerHTML = '<span style="color: green"> Correct </span>'
            } else {
                correctOrNo.innerHTML = '<span style="color: red"> Wrong </span>'
            }

            cloned.id = '';
            cloned.style.display = '';

            answersContainers.appendChild(cloned);
        }

        populateAnsweredSelectableQuestion(question);
    }
}


// Populate answered question
function populateAnsweredSelectableQuestion(question) {
    var type = question.querySelectorAll('input')[0].value;
    var questionAnswers = document.getElementById('questionAnswers');
    var possibleAnswersClassName = type == SINGLE_ANSWER ? '.possible-answer-single' : '.possible-answer';
    var answersContainer = document.querySelectorAll(possibleAnswersClassName);
    var answer = questionAnswers.value.split('|||');


    if (type == SINGLE_ANSWER) {
        for (i = 0; i < answersContainer.length - 1; i++) {
            var answerIdAndIsCorrect = answersContainer[i].querySelectorAll('input')[0].value.split('###');
            var answerId = answerIdAndIsCorrect[0];
            var selected = answersContainer[i].querySelectorAll('input')[1];

            if (answer[0] == answerId) {
                selected.checked = true;

            }

        }
    } else {
        var answers = answer[0].split(',');

        for (i = 0; i < answersContainer.length - 1; i++) {
            var answerIdAndIsCorrect = answersContainer[i].querySelectorAll('input')[0].value.split('###');
            var answerId = answerIdAndIsCorrect[0];
            var selected = answersContainer[i].querySelectorAll('input')[1];

            if (answers.includes(answerId)) {
                selected.checked = true;
            }
        }
    }
}

function addCloseDialogEventListener() {
    var dialog = document.getElementById('closeDialog');
    if (dialog != null) {
        dialog.addEventListener('click', function () {
            document.getElementById('modalResults').style.display = 'none';
        });
    }
}


// Form submission listener
function handleSubmit() {

    var maxPoints = document.getElementById('maxPoints').value
    var givenPoints = document.getElementById('givenPoints');

    if (Number(maxPoints) < Number(givenPoints.value)) {
        document.getElementById('maxPointsError').style.display = '';
        return false;
    }

    return true;

}


populateSelectableQuestions();
addCloseDialogEventListener();
