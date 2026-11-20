# $1 - job name
# $2 - status value

MESSAGE="*$1*: "

if [[ "$2" == "success" ]]; then
  MESSAGE="${MESSAGE} OK ✅"
elif [[ "$2" == "failure" ]]; then
  MESSAGE="${MESSAGE} failed ❌ "
else
  MESSAGE="${MESSAGE} $2 ⚠️"
fi

echo $MESSAGE